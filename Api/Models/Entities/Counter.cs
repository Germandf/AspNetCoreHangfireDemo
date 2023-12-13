using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Api.Models.Entities;

public class Counter
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int CurrentCount { get; set; }
}
