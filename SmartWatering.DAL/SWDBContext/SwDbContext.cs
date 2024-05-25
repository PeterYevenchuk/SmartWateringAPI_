namespace SmartWatering.DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using SmartWatering.DAL.Models;
using SmartWatering.DAL.SWDBContext.Configurations;

public class SwDbContext : DbContext
{
    public SwDbContext(DbContextOptions<SwDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<SensorInformation> SensorInformations { get; set; }
    public DbSet<Watering> Waterings { get; set; }
    public DbSet<MessageModel> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
