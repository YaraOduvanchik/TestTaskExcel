using MoscowWeatherArchives.Domain;

namespace MoscowWeatherArchives.Infrastructure.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WeatherDateConfiguration : IEntityTypeConfiguration<WeatherDate>
{
    public void Configure(EntityTypeBuilder<WeatherDate> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Date)
            .IsRequired();

        builder.HasMany(d => d.WeatherMeasurements)
            .WithOne(m => m.WeatherDate)
            .HasForeignKey(m => m.WeatherDateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}