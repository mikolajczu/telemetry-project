using Telemetry.Entities.Models;
using Telemetry.ViewModels;

namespace Telemetry.Services.Mappers;

public interface ITelemetrySessionMapper
{
    Task<IEnumerable<TelemetrySessionViewModel>> Map(ICollection<TelemetrySession> sessions);
    TelemetrySessionViewModel Map(TelemetrySession session, User user);
}