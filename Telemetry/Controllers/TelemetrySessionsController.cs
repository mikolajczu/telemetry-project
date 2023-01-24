using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Telemetry.Entities.Models;
using Telemetry.ViewModels;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Telemetry.Controllers;

public class TelemetrySessionsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TelemetrySessionsController> _logger;
    private readonly IMongoCollection<User> _users;

    public TelemetrySessionsController(IMongoClient mongoClient, ILogger<TelemetrySessionsController> logger,
        UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;
        var mongoDb = mongoClient.GetDatabase("appdb");
        _users = mongoDb.GetCollection<User>("users");
    }

    ////// GET: TelemetrySessions
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        return View(await MapToTelemetrySessionViewModels(user.Sessions));
    }

    private async Task<List<TelemetrySessionViewModel>> MapToTelemetrySessionViewModels(
        List<TelemetrySession> sessions)
    {
        _logger.LogInformation("BEGGINING MAPPING");

        var userId = sessions.FirstOrDefault()?.UserId;
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        var viewModels = sessions.Select(s => MapToTelemetrySessionViewModel(s, user)).ToList();

        _logger.LogInformation("END MAPPING");

        return viewModels.ToList();
    }

    private TelemetrySessionViewModel MapToTelemetrySessionViewModel(TelemetrySession session, User user)
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
            //Time = session.Time,
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

    ////// GET: TelemetrySessions/Details/5
    [Authorize]
    public async Task<IActionResult> Details(string? id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (id is null)
            return NotFound();

        var telemetrySession = user.Sessions.FirstOrDefault(s => s.Id == ObjectId.Parse(id));

        if (telemetrySession is null)
        {
            return NotFound();
        }

        return View(MapToTelemetrySessionViewModel(telemetrySession, user));
    }

    ////// GET: TelemetrySessions/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(string? id)
    {
        if (id is null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var telemetrySession = user.Sessions.FirstOrDefault(s => s.Id == ObjectId.Parse(id));

        if (telemetrySession is null)
        {
            return NotFound();
        }

        return View(MapToTelemetrySessionViewModel(telemetrySession, user));
    }

    ////// POST: TelemetrySessions/Delete/5
    [HttpPost, ActionName("Delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.GetUserAsync(User);

        var updateDefinition =
            Builders<User>.Update.PullFilter(u => u.Sessions, s => s.Id == ObjectId.Parse(id));

        await _users.FindOneAndUpdateAsync(u => u.Id == user.Id, updateDefinition);

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> CreateNewSession()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var session = new TelemetrySession
        {
            Id = ObjectId.GenerateNewId(),
            UserId = currentUser.Id,
            Status = 1,
            SessionDate = DateTime.UtcNow,
            Pages = new List<Page>()
        };

        var updateDefinition = Builders<User>.Update.Push(u => u.Sessions, session);

        _logger.LogInformation("Before creating new session");
        await _users.UpdateOneAsync(u => u.Id == currentUser.Id, updateDefinition);
        _logger.LogInformation("After creating new session");


        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> ChangeSessionStatus(string id)
    {
        var searchDefinition = Builders<User>.Filter.Eq(u => u.Id, id);
        var sessionUser = await _users.Find(searchDefinition).FirstOrDefaultAsync();
        sessionUser.Sessions.ForEach(s => s.Status = 0);
            

        await _users.UpdateOneAsync(u => u.Id == id, Builders<User>.Update.Set(s => s.Sessions, sessionUser.Sessions));
            
        _logger.LogInformation("Session status changed to 0");

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SendInformationToSession([FromBody] SessionInfoRequest request)
    {
        _logger.LogInformation("Inside SendInformationToSession");

        _logger.LogInformation(JsonSerializer.Serialize(request));
        _logger.LogInformation($"TabTitle: {request.TabTitle}");
        _logger.LogInformation($"TimeSpent: {request.TimeSpent}");

        _logger.LogInformation($"Seconds = {request.TimeSpent}");

        var userId = (await _userManager.GetUserAsync(User)).Id;
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        var session = user.Sessions.First(s => s.Status == 1 && s.UserId == user.Id);

        var generalPage = user.Pages.FirstOrDefault(p => p.Title == request.TabTitle);
        var pageInSession = session.Pages.FirstOrDefault(p => p.Title == request.TabTitle);

        var pageExistsInAccount = generalPage is not null;
        var pageExistsInSession = pageInSession is not null;

        if (!pageExistsInAccount && !pageExistsInSession)
        {
            var accountPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.TabTitle,
                Time = request.TimeSpent / 1000
            };

            var sessionPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.TabTitle,
                Time = request.TimeSpent / 1000
            };

            await _users.UpdateOneAsync(u => u.Id == userId, Builders<User>.Update.Push(u => u.Pages, accountPage));

            session.Pages.Add(sessionPage);

            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.TimeSpent / 1000}");
                
            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateSessionPageDefinition);
        }
        else if (!pageExistsInSession)
        {
            var sessionPage = new Page
            {
                Id = ObjectId.GenerateNewId(),
                Title = request.TabTitle,
                Time = request.TimeSpent / 1000
            };
                
            session.Pages.Add(sessionPage);

            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.FindOneAndUpdateAsync(u => u.Id == user.Id, updateSessionPageDefinition);

            user.Pages.FirstOrDefault(p => p.Title == request.TabTitle).Time += request.TimeSpent / 1000;
            var updateAccountPageDefinition = Builders<User>.Update.Set(u => u.Pages, user.Pages);
            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.TimeSpent / 1000}");

            await _users.FindOneAndUpdateAsync(u => u.Id == user.Id, updateAccountPageDefinition);
        }
        else
        {
            user.Pages.FirstOrDefault(p => p.Title == request.TabTitle).Time += request.TimeSpent / 1000;
            var updateAccountPageDefinition = Builders<User>.Update.Set(u => u.Pages, user.Pages);
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateAccountPageDefinition);
            session.Pages.FirstOrDefault(p => p.Title == request.TabTitle).Time += request.TimeSpent / 1000;
            //session.Time += request.TimeSpent / 1000;
            _logger.LogInformation($"session.Time zwiekszone o {request.TimeSpent / 1000}");
                
            var updateSessionPageDefinition = Builders<User>.Update.Set(u => u.Sessions, user.Sessions);
                
            await _users.UpdateOneAsync(u => u.Id == user.Id, updateSessionPageDefinition);
        }

        return Ok();
    }
}