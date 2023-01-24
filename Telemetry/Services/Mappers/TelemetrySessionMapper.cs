using MongoDB.Driver;
using Telemetry.Entities.Models;
using Telemetry.ViewModels;

namespace Telemetry.Services.Mappers;

public class TelemetrySessionMapper : ITelemetrySessionMapper
{
    private readonly IMongoCollection<User> _users;
    private readonly ILogger<TelemetrySessionMapper> _logger;

    public TelemetrySessionMapper(IMongoClient mongoClient, ILogger<TelemetrySessionMapper> logger)
    {
        _users = mongoClient.GetDatabase("appdb").GetCollection<User>("users");
        _logger = logger;
    }
    public async Task<IEnumerable<TelemetrySessionViewModel>> Map(ICollection<TelemetrySession> sessions)
    {
        var userId = sessions.FirstOrDefault()?.UserId;
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        var viewModels = sessions.Select(s => Map(s, user)).ToList();

        return viewModels.ToList();
    }

    public TelemetrySessionViewModel Map(TelemetrySession session, User user)
    {
        var viewmodel = new TelemetrySessionViewModel
        {
            Id = session.Id.ToString(),
            User = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email
            },
            SessionDate = session.SessionDate,
            Status = session.Status,
            Time = session.Pages.Sum(p => p.Time),
            Pages = session.Pages.Select(p => new PageViewModel
            {
                Id = p.Id.ToString(),
                Title = p.Title,
                Time = p.Time
            }).ToList()
        };

        return viewmodel;
    }
}