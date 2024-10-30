using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Magnise.Web.Handlers;

public class WebSocketHandler
{
    public static async Task HandleClientConnectionAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        var userToken = context.Session.GetString("AccessToken") ?? throw new UnauthorizedAccessException();

        bool isSucceeded = Guid.TryParse(context.Request.RouteValues["id"]?.ToString(), out var id);

        if (!isSucceeded)
        {
            throw new BadHttpRequestException("provide id value");
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleClientConnectionAsync(webSocket, userToken, id);
    }

    private static async Task HandleClientConnectionAsync(WebSocket clientWebSocket, string userToken, Guid instrumentId)
    {
        var buffer = new byte[1024 * 4];
        ClientWebSocket? externalWebSocket = null;

        while (clientWebSocket.State == WebSocketState.Open)
        {
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                continue;
            }

            var clientMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var message = JsonSerializer.Deserialize<MessageModel>(clientMessage);

            if (message?.Type == "start" && externalWebSocket == null)
            {
                externalWebSocket = new ClientWebSocket();
                await ExternalWebSocketHandler.HandleConnectionToPassWebSocketAsync(clientWebSocket, externalWebSocket, userToken);
                await ExternalWebSocketHandler.SubscribeToDataAsync(externalWebSocket, instrumentId);
            }
            else if (message?.Type == "end" && externalWebSocket != null)
            {
                await externalWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended by client", CancellationToken.None);
                externalWebSocket = null;
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended by client", CancellationToken.None);
            }
            else if (message?.Type == "subscribe" && externalWebSocket != null && message.Id.HasValue)
            {
                await ExternalWebSocketHandler.SubscribeToDataAsync(externalWebSocket, message.Id.Value);
            }
        }
    }

    private class MessageModel
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
    }
}
