using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Telemetry.Entities.Models;
using Telemetry.Services.Commands;
using Telemetry.Services.Mappers;
using Telemetry.ViewModels;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Telemetry.Controllers;

public class TelemetrySessionsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TelemetrySessionsController> _logger;
    private readonly IMongoCollection<User> _users;
    private readonly ITelemetrySessionMapper _mapper;
    private readonly ISender _mediator;

    public TelemetrySessionsController(IMongoClient mongoClient, ILogger<TelemetrySessionsController> logger,
        UserManager<User> userManager, ITelemetrySessionMapper mapper, ISender mediator)
    {
        _logger = logger;
        _userManager = userManager;
        var mongoDb = mongoClient.GetDatabase("appdb");
        _users = mongoDb.GetCollection<User>("users");
        _mapper = mapper;
        _mediator = mediator;
    }

    ////// GET: TelemetrySessions
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        return View(await _mapper.Map(user.Sessions));
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

        return View(_mapper.Map(telemetrySession, user));
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

        return View(_mapper.Map(telemetrySession, user));
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
        await _mediator.Send(new SendInformationToSessionCommand(request));
        
        return Ok();
    }
}