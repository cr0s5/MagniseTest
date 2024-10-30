using Contracts.Dtos;

namespace Contracts.Services;

public interface IAssetsService
{
    Task<IReadOnlyList<AssetDto>?> GetAssetsAsync();

    Task<IReadOnlyList<AssetPriceDto>> GetAssetPricesAsync(Guid assetId, DateTime? from, DateTime? to);

    Task UpdateAssetsAsync(string token);

    Task AddAssetPriceAsync(AssetPriceDto dto);
}
