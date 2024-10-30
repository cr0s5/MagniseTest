using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class AssetsDbContext(DbContextOptions<AssetsDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets { get; set; }

    public DbSet<AssetPrice> AssetPrices { get; set; }
}
