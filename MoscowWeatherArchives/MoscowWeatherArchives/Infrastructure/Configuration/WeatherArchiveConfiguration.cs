using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoscowWeatherArchives.Domain;

namespace MoscowWeatherArchives.Infrastructure.Configuration;

public class WeatherArchiveConfiguration : IEntityTypeConfiguration<WeatherArchive>
{
    public void Configure(EntityTypeBuilder<WeatherArchive> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(w => w.Year)
            .IsRequired();

        builder.Property(w => w.UploadDate)
            .IsRequired();

        builder.HasMany(w => w.WeatherDates)
            .WithOne(d => d.WeatherArchive)
            .HasForeignKey(d => d.WeatherArchiveId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}