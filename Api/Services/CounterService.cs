using Api.Models.Entities;
using Api.Models.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Services;

public class CounterService
{
    private readonly IMongoCollection<Counter> _collection;

    public CounterService(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _collection = database.GetCollection<Counter>(nameof(Counter));
    }

    public async Task<int> GetCurrentAndIncrementAsync()
    {
        var document = await _collection.Find(x => true).FirstOrDefaultAsync();
        if (document is null)
        {
            document = new Counter();
            await _collection.InsertOneAsync(document);
            return document.CurrentCount;
        }
        else
        {
            var filter = Builders<Counter>.Filter.Eq("_id", document.Id);
            var update = Builders<Counter>.Update.Inc(x => x.CurrentCount, 1);
            await _collection.UpdateOneAsync(filter, update);
            return document.CurrentCount;
        }
    }
}
