using Microsoft.EntityFrameworkCore;
using MoscowWeatherArchives.Controllers;
using MoscowWeatherArchives.Domain;
using MoscowWeatherArchives.Infrastructure;

namespace MoscowWeatherArchives.Application;

public class GetAllWeatherArchivesWithPaginationHandler
{
    private readonly ApplicationDbContext _dbContext;

    public GetAllWeatherArchivesWithPaginationHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<WeatherMeasurement>> Handle(
        GetAllWeatherArchivesWithPaginationRequest request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.WeatherMeasurements
            .Include(wm => wm.WeatherDate)
            .AsQueryable();

        if (request.Year > 0)
            query = query.Where(wm => wm.WeatherDate.Date.Year == request.Year);

        if (request.Month > 0)
            query = query.Where(wm => wm.WeatherDate.Date.Month == request.Month);

        var totalCount = await query.CountAsync(cancellationToken: cancellationToken);

        var items = await query
            .OrderBy(wm => wm.WeatherDate.Date)
            .ThenBy(wm => wm.Time)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        return new PagedResult<WeatherMeasurement>(items, totalCount, request.Page, request.PageSize);
    }
}