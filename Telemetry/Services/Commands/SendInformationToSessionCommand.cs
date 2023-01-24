using MediatR;
using Telemetry.ViewModels;

namespace Telemetry.Services.Commands;

public class SendInformationToSessionCommand : IRequest<Unit>
{
    public SessionInfoRequest Data { get; set; }
    public SendInformationToSessionCommand(SessionInfoRequest sessionInfoRequest)
    {
        Data = sessionInfoRequest;
    }
}