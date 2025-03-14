using Microsoft.EntityFrameworkCore;
using MoscowWeatherArchives.Domain;

namespace MoscowWeatherArchives.Infrastructure;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    public DbSet<WeatherArchive> WeatherArchives => Set<WeatherArchive>();
    public DbSet<WeatherDate> WeatherDates => Set<WeatherDate>();
    public DbSet<WeatherMeasurement> WeatherMeasurements => Set<WeatherMeasurement>();

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}