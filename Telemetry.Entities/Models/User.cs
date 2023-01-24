using Mongo.AspNetCore.Identity;

namespace Telemetry.Entities.Models;

public class User : MongoIdentityUser
{
    public List<TelemetrySession> Sessions { get; set; } = new();
    public List<Page> Pages { get; set; } = new();
    public string? Description { get; set; }
}