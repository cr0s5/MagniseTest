using Contracts.Dtos;
using Contracts.Services;
using Magnise.Web.Handlers;
using Magnise.Web.Options;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace Magnise.Web.Infrustructure;

public class BackgroundDbUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AdminOptions _adminOptions;

    public BackgroundDbUpdateService(
        IServiceScopeFactory scopeFactory,
        IOptions<AdminOptions> options)
    {
        _scopeFactory = scopeFactory;
        _adminOptions = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var token = await GetTokenAsync(scope);

        if (token == null)
        {
            return;
        }

        var assetsService = scope.ServiceProvider.GetRequiredService<IAssetsService>();

        await assetsService.UpdateAssetsAsync(token);

        await SubscribeToUpdatePricesAsync(token, assetsService);
    }

    private async Task<string?> GetTokenAsync(IServiceScope scope)
    {
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var token = await accountService.GetTokenAsync(new LoginDto { Email = _adminOptions.Email, Password = _adminOptions.Password });

        return token;
    }

    private async Task SubscribeToUpdatePricesAsync(string token, IAssetsService assetsService)
    {
        var assets = await assetsService.GetAssetsAsync();

        if (assets == null || assets.Count == 0)
        {
            return;
        }
        var externalWebSocket = new ClientWebSocket();
        await ExternalWebSocketHandler.HandleConnectionToSaveToDbAsync(externalWebSocket, token, _scopeFactory);

        foreach (var asset in assets)
        {
            await ExternalWebSocketHandler.SubscribeToDataAsync(externalWebSocket, asset.Id);
        }
    }
}