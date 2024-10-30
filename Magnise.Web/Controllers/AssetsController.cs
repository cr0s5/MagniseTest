using AutoMapper;
using Contracts.Services;
using Magnise.Web.Infrustructure;
using Magnise.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magnise.Web.Controllers;

[SessionAuthorize]
[ApiController]
[Route("api/assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsService _assetsService;
    private readonly IMapper _mapper;

    public AssetsController(IAssetsService assetsService, IMapper mapper)
    {
        _assetsService = assetsService;
        _mapper = mapper;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAvailableAssets()
    {
        var dtos = await _assetsService.GetAssetsAsync();

        var model = _mapper.Map<List<AssetModel>>(dtos);

        return Ok(model);
    }

    [HttpGet("history-prices/{id}")]
    public async Task<IActionResult> GetAssetPrices(Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var dtos = await _assetsService.GetAssetPricesAsync(id, from, to);

        var model = _mapper.Map<List<AssetPriceModel>>(dtos);

        return Ok(model);
    }
}
