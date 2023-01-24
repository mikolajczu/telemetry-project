using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry.Entities.Models;

public class TelemetrySession
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string UserId { get; set; }

    public  ICollection<Page> Pages { get; set; } 
    public DateTime SessionDate { get; set; }
    //public double Time { get; set; }
    public byte Status { get; set; } // 0 - OFF, 1 - ON
}