using Flustri.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core.Queries;

public record ListRoomsQueryRequest (
    int Page,
    int PageSize
);

public static class ListRoomsQuery
{    
    public static async Task<List<Room>> ConsumeAsync(ListRoomsQueryRequest query, FlustriDbContext db)
    {
        return await db.Rooms
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
    }
}