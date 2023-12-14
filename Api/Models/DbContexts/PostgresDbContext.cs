using Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Models.DbContexts;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }

    public DbSet<Counter> Counters { get; set; }
}
