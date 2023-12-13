using Api.Filters;
using Api.Models.Consts;
using Api.Models.Settings;
using Api.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<HangfireDocumentFilter>();
});
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMongoStorage(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName, new MongoStorageOptions
        {
            MigrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            },
        })
        .UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();
builder.Services.AddSingleton<CounterService>();

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
