using Contracts.Dtos;
using Contracts.Providers;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Contracts.Impl.Providers;

public class AssetsProvider : IAssetsProvider
{
    private const string Url = "https://platform.fintacharts.com/api/instruments/v1/instruments?provider=oanda&kind=forex";

    public async Task<IReadOnlyList<AssetDto>?> ProvideAsync(string token)
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(Url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return apiResponse?.Data;
    }

    private class ApiResponse
    {    
        public required List<AssetDto> Data { get; init; }
    }
}
