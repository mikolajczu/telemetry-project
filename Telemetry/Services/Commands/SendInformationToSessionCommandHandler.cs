using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using Telemetry.Entities.Models;
using Telemetry.ViewModels;

namespace Telemetry.Services.Commands;

public class SendInformationToSessionCommandHandler : IRequestHandler<SendInformationToSessionCommand, Unit>
{
    private readonly ILogger<SendInformationToSessionCommandHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMongoCollection<User> _users;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public SendInformationToSessionCommandHandler(ILogger<SendInformationToSessionCommandHandler> logger, UserManager<User> userManager, IMongoClient mongoClient, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userManager = userManager;
        _users = mongoClient.GetDatabase("appdb").GetCollection<User>("users");
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Unit> Handle(SendInformationToSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Inside SendInformationToSession");

        _logger.LogInformation(JsonSerializer.Serialize(request));
        _logger.LogInformation($"TabTitle: {request.Data.TabTitle}");
        _logger.LogInformation($"TimeSpent: {request.Data.TimeSpent}");

        _logger.LogInformation($"Seconds = {request.Data.TimeSpent}");

        var userId = (await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User)).Id;
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        var session = user.Sessions.First(s => s.Status == 1 && s.UserId == user.Id);

        var generalPage = user.Pages.FirstOrDefault(p => p.Title == request.Data.TabTitle);
        var pageInSession = session.Pages.FirstOrDefault(p => p.Title == request.Data.TabTitle);

        var pageExistsInAccount = generalPage is not null;
        var pageExistsInSession = pageInSession is not null;

        if (!pageExistsInAccount && !pageExistsInSession)
        {
            var accountPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.Data.TabTitle,
                Time = request.Data.TimeSpent / 1000
            };

            var sessionPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.Data.TabTitle,
                Time = request.Data.TimeSpent / 1000
            };

            await _users.UpdateOneAsync(u => u.Id == userId, Builders<User>.Update.Push(u => u.Pages, accountPage));

            session.Pages.Add(sessionPage);

            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.Data.TimeSpent / 1000}");
                
            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateSessionPageDefinition);
        }
        else if (!pageExistsInSession)
        {
            var sessionPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.Data.TabTitle,
                Time = request.Data.TimeSpent / 1000
            };
                
            session.Pages.Add(sessionPage);

            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.FindOneAndUpdateAsync(u => u.Id == user.Id, updateSessionPageDefinition);

            user.Pages.FirstOrDefault(p => p.Title == request.Data.TabTitle).Time += request.Data.TimeSpent / 1000;
            var updateAccountPageDefinition = Builders<User>.Update.Set(u => u.Pages, user.Pages);
            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.Data.TimeSpent / 1000}");

            await _users.FindOneAndUpdateAsync(u => u.Id == user.Id, updateAccountPageDefinition);
        }
        else
        {
            user.Pages.FirstOrDefault(p => p.Title == request.Data.TabTitle).Time += request.Data.TimeSpent / 1000;
            var updateAccountPageDefinition = Builders<User>.Update.Set(u => u.Pages, user.Pages);
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateAccountPageDefinition);
            session.Pages.FirstOrDefault(p => p.Title == request.Data.TabTitle).Time += request.Data.TimeSpent / 1000;
            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.Data.TimeSpent / 1000}");
                
            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateSessionPageDefinition);
        }
        
        return Unit.Value;
    }
}