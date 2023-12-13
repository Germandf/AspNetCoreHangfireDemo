using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Models.Entities;

public class Counter
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int CurrentCount { get; set; }
}
