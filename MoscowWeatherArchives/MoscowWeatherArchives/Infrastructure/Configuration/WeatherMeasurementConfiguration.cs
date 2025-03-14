using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoscowWeatherArchives.Domain;

namespace MoscowWeatherArchives.Infrastructure.Configuration;

public class WeatherMeasurementConfiguration : IEntityTypeConfiguration<WeatherMeasurement>
{
    public void Configure(EntityTypeBuilder<WeatherMeasurement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Time)
            .IsRequired();

        builder.Property(m => m.Temperature);

        builder.Property(m => m.RelativeHumidity);

        builder.Property(m => m.DewPoint);

        builder.Property(m => m.AtmosphericPressure);

        builder.Property(m => m.WindDirection)
            .HasMaxLength(30);

        builder.Property(m => m.WindSpeed);

        builder.Property(m => m.Cloudiness);

        builder.Property(m => m.CloudBaseHeight);

        builder.Property(m => m.VerticalVisibility);

        builder.Property(m => m.WeatherPhenomena)
            .HasMaxLength(255);
    }
}