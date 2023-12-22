using Microsoft.EntityFrameworkCore;
using ShopifyAppTemplate.Models;

namespace ShopifyAppTemplate.Data;

public class ShopjetsDataContext : DbContext
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=ShopJetsLocal.db");
    }
    
    //User context entities
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<OAuthState> LoginStates { get; set; } = null!;
}