namespace MoscowWeatherArchives.Application;

public record GetAllWeatherArchivesWithPaginationRequest(int? Year = 0, int? Month = 0, int Page = 1, int PageSize = 10);