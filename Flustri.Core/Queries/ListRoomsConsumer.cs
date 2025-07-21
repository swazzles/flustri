using Flustri.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Flustri.Core.Queries;

public record ListRoomsQuery (
    int Page,
    int PageSize
);

public static class ListRoomsConsumer
{    
    public static async Task<List<Room>> ConsumeAsync(ListRoomsQuery query, FlustriDbContext db)
    {
        return await db.Rooms
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
    }
}