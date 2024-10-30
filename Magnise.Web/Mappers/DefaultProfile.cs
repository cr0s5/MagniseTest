using AutoMapper;
using Contracts.Dtos;
using Magnise.Web.Models;

namespace Magnise.Web.Mappers;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        CreateMap<LoginModel, LoginDto>();

        CreateMap<AssetDto, AssetModel>();

        CreateMap<AssetPriceDto, AssetPriceModel>();

        CreateMap<AssetPriceModel, AssetPriceDto>();
    }
}
