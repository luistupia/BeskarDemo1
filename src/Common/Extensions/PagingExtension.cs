using Common.Base;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

public static class PagingExtension
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
        int page, int pageSize) where T : class
    {
        var result = new PagedResult<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = query.Count()
        };

        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = await query.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();
        return result;
    }
}