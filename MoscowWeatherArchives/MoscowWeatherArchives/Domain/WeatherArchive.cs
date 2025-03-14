using CSharpFunctionalExtensions;

namespace MoscowWeatherArchives.Domain;

public class WeatherArchive : Entity<Guid>
{
    private readonly List<WeatherDate> _weatherDates;

    // Ef core constructor
    private WeatherArchive()
    {
    }

    public WeatherArchive(Guid id, string fileName, int year, DateTime uploadDate)
        : base(id)
    {
        FileName = fileName;
        Year = year;
        UploadDate = uploadDate;
        _weatherDates = new List<WeatherDate>();
    }

    public string FileName { get; private set; }
    public int Year { get; private set; }
    public DateTime UploadDate { get; private set; }

    public IReadOnlyCollection<WeatherDate> WeatherDates => _weatherDates.AsReadOnly();

    public UnitResult<string> AddWeatherDate(WeatherDate weatherDate)
    {
        if (_weatherDates.Any(d => d.Date == weatherDate.Date))
            return Result.Failure($"Дата {weatherDate.Date} уже существует в архиве.");

        _weatherDates.Add(weatherDate);
        return Result.Success();
    }
}