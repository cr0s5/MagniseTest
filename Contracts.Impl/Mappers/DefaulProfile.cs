using AutoMapper;
using Contracts.Dtos;
using DataAccess.Models;

namespace Contracts.Impl.Mappers;

public class DefaulProfile : Profile
{
    public DefaulProfile()
    {
        CreateMap<AssetPriceDto, AssetPrice>();

        CreateMap<AssetPrice, AssetPriceDto>();
    }
}