﻿namespace MoscowWeatherArchives.Controllers;

public class PagedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    public PagedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page * PageSize < TotalCount;
}