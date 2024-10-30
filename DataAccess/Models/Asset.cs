namespace DataAccess.Models;

public class Asset
{
    public Guid Id { get; set; }

    public required string Symbol { get; set; }

    public required string Description { get; set; }

    public ICollection<AssetPrice>? Prices { get; set; }
}
