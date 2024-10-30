using Contracts.Dtos;
using Contracts.Services;
using Newtonsoft.Json.Linq;

namespace Contracts.Impl.Services;

public class AccountService : IAccountService
{
    private const string Url = "https://platform.fintacharts.com/identity/realms/fintatech/protocol/openid-connect/token";

    public async Task<string?> GetTokenAsync(LoginDto dto)
    {
        var formData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", "app-cli" },
            { "username", dto.Email },
            { "password", dto.Password }
        };

        using var client = new HttpClient();

        var content = new FormUrlEncodedContent(formData);

        var response = await client.PostAsync(Url, content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JObject.Parse(responseContent);
        return jsonResponse["access_token"]?.ToString();
    }
}
