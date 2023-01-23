using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry.Entities.Models;

public class UserPage
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string PageId { get; set; }
    public string TelemetrySessionId { get; set; }
    public double Time { get; set; }
}