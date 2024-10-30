using AutoMapper;
using Contracts.Dtos;
using Contracts.Providers;
using Contracts.Services;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Contracts.Impl.Services;

public class AssetsService : IAssetsService
{
    private readonly AssetsDbContext _dbContext;
    private readonly IAssetsProvider _assetsProvider;
    private readonly IMapper _mapper;

    public AssetsService(
        AssetsDbContext dbContext,
        IAssetsProvider assetsProvider,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _assetsProvider = assetsProvider;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AssetDto>?> GetAssetsAsync()
    {
        var assets = await _dbContext.Set<Asset>().ToListAsync();

        return assets.Select(x => new AssetDto
        {
            Id = x.Id,
            Symbol = x.Symbol,
            Description = x.Description
        }).ToList();
    }

    public async Task<IReadOnlyList<AssetPriceDto>> GetAssetPricesAsync(Guid assetId, DateTime? from, DateTime? to)
    {
        var query = _dbContext.Set<AssetPrice>()
            .Where(x => x.AssetId == assetId);

        if (from.HasValue)
        {
            query = query.Where(x => from <= x.TimeStamp);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.TimeStamp <= to);
        }

        var assetPrices = await query
            .OrderByDescending(x => x.TimeStamp)
            .ToListAsync();

        return _mapper.Map<IReadOnlyList<AssetPriceDto>>(assetPrices);
    }

    public async Task UpdateAssetsAsync(string token)
    {
        var dtos = await _assetsProvider.ProvideAsync(token);

        if (dtos == null || dtos.Count == 0)
        {
            return;
        }

        var assets = await _dbContext.Set<Asset>().ToListAsync();

        await UpdateAssetsAsync(assets, dtos);
    }

    public async Task AddAssetPriceAsync(AssetPriceDto dto)
    {
        var assetPrice = new AssetPrice
        {
            Id = Guid.NewGuid()
        };

        _mapper.Map(dto, assetPrice);

        _dbContext.Add(assetPrice);
        await _dbContext.SaveChangesAsync();
    }

    private async Task UpdateAssetsAsync(IList<Asset> assets, IReadOnlyList<AssetDto> dtos)
    {
        var dtoIds = dtos.Select(x => x.Id).ToList();
        var assetIds = assets.Select(x => x.Id).ToList();

        var assetsToAdd = dtos.Where(x => !assetIds.Contains(x.Id))
            .Select(x => new Asset
            {
                Id = x.Id,
                Symbol = x.Symbol,
                Description = x.Description
            })
            .ToList();

        var assetsToRemove = assets.Where(x => !dtoIds.Contains(x.Id)).ToList();
        var assetsToUpdate = assets.Where(x => dtoIds.Contains(x.Id)).ToList();

        _dbContext.RemoveRange(assetsToRemove);
        _dbContext.AddRange(assetsToAdd);

        foreach (var assetToUpdate in assetsToUpdate)
        {
            var dto = dtos.First(x => x.Id == assetToUpdate.Id);

            assetToUpdate.Symbol = dto.Symbol;
            assetToUpdate.Description = dto.Description;
        }

        await _dbContext.SaveChangesAsync();
    }
}
