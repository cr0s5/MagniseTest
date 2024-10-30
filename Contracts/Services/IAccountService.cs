using Contracts.Dtos;

namespace Contracts.Services;

public interface IAccountService
{
    Task<string?> GetTokenAsync(LoginDto dto);
}
