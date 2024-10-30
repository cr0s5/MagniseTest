using Contracts.Dtos;

namespace Contracts.Providers;

public interface IAssetsProvider
{
    Task<IReadOnlyList<AssetDto>?> ProvideAsync(string token);
}
