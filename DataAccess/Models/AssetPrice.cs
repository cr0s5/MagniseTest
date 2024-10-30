using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public class AssetPrice
{
    public Guid Id { get; set; }

    public DateTime TimeStamp { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AskPrice { get; set; }

    public int? AskVolume { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BidPrice { get; set; }

    public int? BidVolume { get; set; }

    public Guid AssetId { get; set; }

    public Asset? Asset { get; set; }
}
