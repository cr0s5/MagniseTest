using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using Magnise.Web.Models;
using Contracts.Services;
using Contracts.Dtos;
using AutoMapper;

namespace Magnise.Web.Handlers;

public class ExternalWebSocketHandler
{
    private const string Url = "wss://platform.fintacharts.com/api/streaming/ws/v1/realtime?token=";

    public static async Task HandleConnectionToPassWebSocketAsync(WebSocket clientWebSocket, ClientWebSocket externalWebSocket, string userToken)
    {
        await externalWebSocket.ConnectAsync(new Uri(Url + userToken), CancellationToken.None);

        _ = Task.Run(async () => await RelayMessagesAsync(clientWebSocket, externalWebSocket));
    }

    public static async Task HandleConnectionToSaveToDbAsync(ClientWebSocket externalWebSocket, string userToken, IServiceScopeFactory scopeFactory)
    {
        await externalWebSocket.ConnectAsync(new Uri(Url + userToken), CancellationToken.None);

        _ = Task.Run(async () => await SaveMessagesAsync(externalWebSocket, scopeFactory));
    }

    private static async Task RelayMessagesAsync(WebSocket clientWebSocket, WebSocket externalWebSocket)
    {
        var buffer = new byte[1024 * 4];

        while (externalWebSocket.State == WebSocketState.Open && clientWebSocket.State == WebSocketState.Open)
        {
            var result = await externalWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                continue;
            }

            var jsonContent = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var model = GetPriceModel(jsonContent);

            if (model != null)
            {
                var jsonModel = JsonSerializer.Serialize(model, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
                await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonModel)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private static async Task SaveMessagesAsync(WebSocket externalWebSocket, IServiceScopeFactory scopeFactory)
    {
        var buffer = new byte[1024 * 4];

        while (externalWebSocket.State == WebSocketState.Open)
        {
            var result = await externalWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                continue;
            }

            var jsonContent = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var model = GetPriceModel(jsonContent);

            if (model != null)
            {
                using var scope = scopeFactory.CreateScope();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var assetsService = scope.ServiceProvider.GetRequiredService<IAssetsService>();

                var dto = mapper.Map<AssetPriceDto>(model);

                await assetsService.AddAssetPriceAsync(dto);
            }
        }
    }

    public static async Task SubscribeToDataAsync(WebSocket socket, Guid instrumentId)
    {
        var model = new SubscribeModel
        {
            Type = "l1-subscription",
            Id = "1",
            InstrumentId = instrumentId,
            Provider = "simulation",
            Subscribe = true,
            Kinds = ["ask", "bid", "last"]
        };

        string json = JsonSerializer.Serialize(model);

        var messageBytes = Encoding.UTF8.GetBytes(json);
        var buffer = new ArraySegment<byte>(messageBytes);
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static AssetPriceModel? GetPriceModel(string jsonContent)
    {
        var message = JsonSerializer.Deserialize<MessageModel>(jsonContent);

        var item = message?.Ask ?? message?.Bid;

        if (message?.Type != "l1-update" || item == null)
        {
            return null;
        }

        return new AssetPriceModel
        {
            TimeStamp = item!.Timestamp,
            AskPrice = message.Ask?.Price,
            AskVolume = message.Ask?.Volume,
            BidPrice = message.Bid?.Price,
            BidVolume = message.Bid?.Volume,
            AssetId = message.InstrumentId.GetValueOrDefault()
        };
    }

    private class SubscribeModel
    {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("instrumentId")]
        public Guid InstrumentId { get; init; }

        [JsonPropertyName("provider")]
        public required string Provider { get; init; }

        [JsonPropertyName("subscribe")]
        public bool Subscribe { get; init; }

        [JsonPropertyName("kinds")]
        public required string[] Kinds { get; init; }
    }

    private class MessageModel
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("instrumentId")]
        public Guid? InstrumentId { get; set; }

        [JsonPropertyName("ask")]
        public ItemModel? Ask { get; set; }

        [JsonPropertyName("bid")]
        public ItemModel? Bid { get; set; }
    }

    private class ItemModel
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("volume")]
        public int Volume { get; set; }
    }
}
