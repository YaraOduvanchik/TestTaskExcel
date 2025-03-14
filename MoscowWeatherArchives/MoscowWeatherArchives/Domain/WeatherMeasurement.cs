using CSharpFunctionalExtensions;

namespace MoscowWeatherArchives.Domain;

public class WeatherMeasurement : Entity<Guid>
{
    // Ef core constructor
    private WeatherMeasurement()
    {
    }

    public WeatherMeasurement(
        Guid id,
        TimeSpan time,
        double? temperature,
        byte? relativeHumidity,
        double? dewPoint,
        int? atmosphericPressure,
        string windDirection,
        int? windSpeed,
        byte? cloudiness,
        int? cloudBaseHeight,
        int? verticalVisibility,
        string weatherPhenomena,
        Guid weatherDateId,
        WeatherDate weatherDate) : base(id)
    {
        Time = time;
        Temperature = temperature;
        RelativeHumidity = relativeHumidity;
        DewPoint = dewPoint;
        AtmosphericPressure = atmosphericPressure;
        WindDirection = windDirection;
        WindSpeed = windSpeed;
        Cloudiness = cloudiness;
        CloudBaseHeight = cloudBaseHeight;
        VerticalVisibility = verticalVisibility;
        WeatherPhenomena = weatherPhenomena;
        WeatherDateId = weatherDateId;
        WeatherDate = weatherDate;
    }

    /// <summary>
    /// Время измерения в течение дня.
    /// </summary>
    public TimeSpan Time { get; private set; }

    /// <summary>
    /// Температура воздуха (Т).
    /// </summary>
    public double? Temperature { get; private set; }

    /// <summary>
    /// Относительная влажность воздуха, %.
    /// </summary>
    public byte? RelativeHumidity { get; private set; }

    /// <summary>
    /// Температура точки росы (Td).
    /// </summary>
    public double? DewPoint { get; private set; }

    /// <summary>
    /// Атмосферное давление, мм рт.ст.
    /// </summary>
    public int? AtmosphericPressure { get; private set; }

    /// <summary>
    /// Направление ветра.
    /// </summary>
    public string WindDirection { get; private set; }

    /// <summary>
    /// Скорость ветра, м/с.
    /// </summary>
    public int? WindSpeed { get; private set; }

    /// <summary>
    /// Облачность в процентах.
    /// </summary>
    public byte? Cloudiness { get; private set; }

    /// <summary>
    /// Высота облачного основания (h).
    /// </summary>
    public int? CloudBaseHeight { get; private set; }

    /// <summary>
    /// Вертикальная видимость (VV).
    /// </summary>
    public int? VerticalVisibility { get; private set; }

    /// <summary>
    /// Погодные явления.
    /// </summary>
    public string WeatherPhenomena { get; private set; }

    /// <summary>
    /// Внешний ключ к записи WeatherDate.
    /// </summary>
    public Guid WeatherDateId { get; private set; }

    public WeatherDate WeatherDate { get; private set; }
}