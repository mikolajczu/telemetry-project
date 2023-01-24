using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry.Entities.Models;

public class Page
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Title { get; set; } = null!;

    public double Time { get; set; }
}