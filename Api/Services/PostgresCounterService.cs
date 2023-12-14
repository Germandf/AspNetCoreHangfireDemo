using Api.Models.DbContexts;
using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PostgresCounterService : ICounterService
{
    private readonly PostgresDbContext _dbContext;

    public PostgresCounterService(PostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetCurrentAndIncrementAsync()
    {
        var counter = await _dbContext.Counters.FirstOrDefaultAsync();
        if (counter is null)
        {
            counter = new Counter();
            _dbContext.Add(counter);
            await _dbContext.SaveChangesAsync();
            return counter.CurrentCount;
        }
        else
        {
            counter.CurrentCount++;
            await _dbContext.SaveChangesAsync();
            return counter.CurrentCount;
        }
    }
}
