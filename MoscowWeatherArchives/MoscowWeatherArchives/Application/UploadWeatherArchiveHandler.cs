using System.Globalization;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using MoscowWeatherArchives.Domain;
using MoscowWeatherArchives.Infrastructure;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MoscowWeatherArchives.Application;

public class UploadWeatherArchiveHandler
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UploadWeatherArchiveHandler> _logger;

    public UploadWeatherArchiveHandler(
        ApplicationDbContext context, 
        ILogger<UploadWeatherArchiveHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(Stream fileStream, string fileName, CancellationToken cancellationToken)
{
    using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    try
    {
        var workbook = GetWorkbook(fileStream, fileName);
        var year = ExtractYearFromFileName(fileName);

        if (!year.HasValue)
        {
            _logger.LogWarning("Не удалось определить год из имени файла: {FileName}", fileName);
            return Result.Failure("Не удалось определить год из имени файла");
        }

        var weatherArchive = new WeatherArchive(Guid.NewGuid(), fileName, year.Value, DateTime.UtcNow);
        var existingArchive = await _context.WeatherArchives
            .FirstOrDefaultAsync(a => a.FileName == fileName && a.Year == year.Value, cancellationToken);

        if (existingArchive != null)
        {
            _logger.LogWarning("Архив с таким именем и годом уже существует: {FileName}, {Year}", fileName, year.Value);
            return Result.Failure("Архив с таким именем и годом уже существует");
        }

        for (var sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
        {
            var sheet = workbook.GetSheetAt(sheetIndex);
            var dataRows = ExtractDataRows(sheet);
            if (dataRows is null) continue;

            foreach (var row in dataRows)
            {
                var date = ParseDate(row.GetCell(0));
                if (!date.HasValue) continue; // Пропускаем строки с невалидной датой

                var time = ParseTime(row.GetCell(1));
                var temperature = ParseDouble(row.GetCell(2));
                var humidity = ParseByte(row.GetCell(3));
                var dewPoint = ParseDouble(row.GetCell(4));
                var pressure = ParseInt(row.GetCell(5));
                var windDirection = ParseWindDirection(row.GetCell(6));
                var windSpeed = ParseInt(row.GetCell(7));
                var cloudiness = ParseByte(row.GetCell(8));
                var cloudBase = ParseInt(row.GetCell(9));
                var visibility = ParseVisibility(row.GetCell(10));
                var phenomena = ParseCellString(row.GetCell(11));

                var weatherDate = weatherArchive.WeatherDates
                    .FirstOrDefault(d => d.Date == date.Value) ??
                    new WeatherDate(Guid.NewGuid(), date.Value);

                var measurement = new WeatherMeasurement(
                    Guid.NewGuid(),
                    time,
                    temperature,
                    humidity,
                    dewPoint,
                    pressure,
                    windDirection,
                    windSpeed,
                    cloudiness,
                    cloudBase,
                    visibility,
                    phenomena,
                    weatherDate.Id,
                    weatherDate);

                if (!weatherArchive.WeatherDates.Contains(weatherDate))
                    weatherArchive.AddWeatherDate(weatherDate);

                weatherDate.AddMeasurement(measurement);
            }
        }

        await _context.WeatherArchives.AddAsync(weatherArchive, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        _logger.LogInformation("Архив успешно загружен: {FileName}, {Year}", fileName, year.Value);

        return Result.Success();
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(cancellationToken);
        _logger.LogError(ex, "Ошибка при загрузке архива: {FileName}", fileName);

        return Result.Failure("Формат файла неправильный!");
    }
}

    private static IWorkbook GetWorkbook(Stream stream, string fileName)
    {
        stream.Position = 0;
        return fileName.EndsWith(".xls")
            ? new HSSFWorkbook(stream)
            : new XSSFWorkbook(stream);
    }

    private static List<IRow> ExtractDataRows(ISheet sheet)
    {
        var dataRows = new List<IRow>();

        // Проверяем наличие заголовка
        var headerRow = sheet.GetRow(0);
        if (headerRow == null ) return null;

        // Начинаем с первой строки данных (индекс 1)
        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);
            if (row == null || row.Cells.All(c => c == null || c.CellType == CellType.Blank)) continue;
            dataRows.Add(row);
        }

        return dataRows;
    }

    private static DateTime? ParseDate(ICell cell)
    {
        if (cell == null) return null;
        var value = cell.StringCellValue;
        if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
        return null;
    }

    private static TimeSpan ParseTime(ICell cell)
    {
        if (cell == null) return TimeSpan.Zero;
        var value = cell.StringCellValue;
        return TimeSpan.TryParse(value, out var time) ? time : TimeSpan.Zero;
    }

    private static double? ParseDouble(ICell cell)
    {
        if (cell == null || cell.CellType == CellType.Blank) return null;
        if (cell.CellType == CellType.Numeric) return cell.NumericCellValue;
        if (cell.CellType == CellType.String)
        {
            var value = cell.StringCellValue.Replace(',', '.');
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : null;
        }

        return null;
    }

    private static byte? ParseByte(ICell cell)
    {
        if (cell == null || cell.CellType == CellType.Blank) return null;
        if (cell.CellType == CellType.Numeric)
        {
            var value = cell.NumericCellValue;
            return value >= 0 && value <= 255 ? (byte?)value : null;
        }

        return null;
    }

    private static int? ParseInt(ICell cell)
    {
        if (cell == null || cell.CellType == CellType.Blank) return null;
        if (cell.CellType == CellType.Numeric)
        {
            var value = cell.NumericCellValue;
            return value >= 0 && value <= int.MaxValue ? (int?)value : null;
        }

        return null;
    }

    private static string ParseCellString(ICell cell)
    {
        if (cell == null) return string.Empty;

        return cell.CellType switch
        {
            CellType.String => cell.StringCellValue.Trim(),
            CellType.Numeric => cell.NumericCellValue.ToString(),
            CellType.Boolean => cell.BooleanCellValue.ToString(),
            CellType.Formula => cell.CellFormula,
            _ => string.Empty
        };
    }
    
    private static int? ParseVisibility(ICell cell)
    {
        if (cell == null || cell.CellType == CellType.Blank) return null;
        var value = ParseCellString(cell);
        if (int.TryParse(value, out int result))
            return result;

        // Обработка текстовых значений (например, "менее 100 м")
        var match = Regex.Match(value, @"\d+");
        return match.Success ? int.Parse(match.Value) : null;
    }

    private static string ParseWindDirection(ICell cell)
    {
        return ParseCellString(cell);
    }

    private static int? ExtractYearFromFileName(string fileName)
    {
        var match = Regex.Match(fileName, @"\d{4}");
        return match.Success ? int.Parse(match.Value) : null;
    }
}