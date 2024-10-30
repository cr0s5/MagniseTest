namespace Magnise.Web.Models;

public class AssetPriceModel
{
    public DateTime TimeStamp { get; init; }

    public decimal? AskPrice { get; init; }

    public int? AskVolume { get; init; }

    public decimal? BidPrice { get; init; }

    public int? BidVolume { get; init; }

    public Guid AssetId { get; init; }
}
