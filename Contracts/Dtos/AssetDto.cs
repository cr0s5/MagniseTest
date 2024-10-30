namespace Contracts.Dtos;

public class AssetDto
{
    public Guid Id { get; init; }

    public required string Symbol { get; init; }

    public required string Description { get; init; }
}
