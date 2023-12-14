using Api.Filters;
using Api.Models.Consts;
using Api.Models.DbContexts;
using Api.Models.Settings;
using Api.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// Change this to use one of the storage implementations available
var storageImplementation = StorageImplementation.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<HangfireDocumentFilter>();
});
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));
builder.Services.AddORMs(storageImplementation);
builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseColouredConsoleLogProvider()
        .WithStorage(dbSettings, storageImplementation);
});
builder.Services.AddHangfireServer();
builder.Services.AddCounterService(storageImplementation);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHangfireDashboard(new DashboardOptions
{
    Authorization = new[] { new DashboardAuthorizationFilter() },
    AppPath = "/swagger",
    IsReadOnlyFunc = (DashboardContext context) => true,
});

RecurringJob.AddOrUpdate<DoSomethingService>(RecurringJobNames.DoSomething, x => x.DoSomethingAsync(), "*/15 * * * * *");

app.Run();

public static partial class Program
{
    public enum StorageImplementation
    {
        Mongo,
        Postgres
    }

    public static IServiceCollection AddORMs(this IServiceCollection services,
               StorageImplementation storageImplementation)
    {
        if (storageImplementation == StorageImplementation.Postgres)
        {
            services = services.AddDbContext<PostgresDbContext>((sp, options) =>
            {
                var dbSettings = sp.GetRequiredService<IOptions<DbSettings>>().Value;
                options.UseNpgsql($"{dbSettings.PostgresConnectionString}Database={dbSettings.DatabaseName};");
            });
            var postgresDbContext = services.BuildServiceProvider().GetRequiredService<PostgresDbContext>();
            postgresDbContext.Database.EnsureCreated();
        }

        return services;
    }

    public static IGlobalConfiguration WithStorage(this IGlobalConfiguration configuration, 
        DbSettings dbSettings, StorageImplementation storageImplementation)
    {
        if (storageImplementation == StorageImplementation.Mongo)
        {
            configuration = configuration.UseMongoStorage(dbSettings.MongoConnectionString, dbSettings.DatabaseName, new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    BackupStrategy = new CollectionMongoBackupStrategy()
                },
            });
        }
        else if (storageImplementation == StorageImplementation.Postgres)
        {
            configuration = configuration.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection($"{dbSettings.PostgresConnectionString}Database={dbSettings.DatabaseName};");
            });
        }

        return configuration;
    }

    public static IServiceCollection AddCounterService(this IServiceCollection services, 
        StorageImplementation storageImplementation)
    {
        if (storageImplementation == StorageImplementation.Mongo)
        {
            services = services.AddSingleton<ICounterService, MongoCounterService>();
        }
        else if (storageImplementation == StorageImplementation.Postgres)
        {
            services = services.AddScoped<ICounterService, PostgresCounterService>();
        }

        return services;
    }
}
