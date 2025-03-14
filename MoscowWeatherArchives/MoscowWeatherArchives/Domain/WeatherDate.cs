using CSharpFunctionalExtensions;

namespace MoscowWeatherArchives.Domain;

public class WeatherDate : Entity<Guid>
{
    private readonly List<WeatherMeasurement> _weatherMeasurements;

    // Ef core constructor
    private WeatherDate()
    {
    }

    public WeatherDate(Guid id, DateTime date) : base(id)
    {
        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        _weatherMeasurements = new List<WeatherMeasurement>();
    }

    public DateTime Date { get; private set; }
    public IReadOnlyCollection<WeatherMeasurement> WeatherMeasurements => _weatherMeasurements.AsReadOnly();

    /// <summary>
    /// Внешний ключ к записи WeatherArchive
    /// </summary>
    public Guid WeatherArchiveId { get; private set; }

    public WeatherArchive WeatherArchive { get; private set; }

    public void AddMeasurement(WeatherMeasurement measurement)
    {
        _weatherMeasurements.Add(measurement);
    }
}