namespace Magnise.Web.Models;

public class AssetModel
{
    public Guid Id { get; init; }

    public required string Symbol { get; init; }

    public required string Description { get; init; }
}
