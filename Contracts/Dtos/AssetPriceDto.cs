namespace Contracts.Dtos;

public class AssetPriceDto
{
    public DateTime TimeStamp { get; init; }

    public decimal? AskPrice { get; init; }

    public int? AskVolume { get; init; }

    public decimal? BidPrice { get; init; }

    public int? BidVolume { get; init; }

    public Guid AssetId { get; init; }
}
