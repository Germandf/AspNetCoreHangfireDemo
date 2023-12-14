namespace Api.Models.Settings;

public class DbSettings
{
    public required string MongoConnectionString { get; set; }
    public required string PostgresConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
