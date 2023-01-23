using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telemetry.Entities;
using Telemetry.Entities.Models;
using Telemetry.ViewModels;

namespace Telemetry.Controllers
{
    public class TelemetrySessionsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TelemetrySessionsController> _logger;
        private readonly IMongoCollection<TelemetrySession> _telemetrySessions;
        private readonly IMongoCollection<Page> _pages;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserPage> _userPages;

        public TelemetrySessionsController(IMongoClient mongoClient, ILogger<TelemetrySessionsController> logger,
            UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            var mongoDb = mongoClient.GetDatabase("appdb");
            _telemetrySessions = mongoDb.GetCollection<TelemetrySession>("telemetrySessions");
            _pages = mongoDb.GetCollection<Page>("pages");
            _users = mongoDb.GetCollection<User>("users");
            _userPages = mongoDb.GetCollection<UserPage>("userPages");
        }

        //// GET: TelemetrySessions
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var sessions = await _telemetrySessions
                .Find(_ => true)
                .ToListAsync(cancellationToken);


            return View(await MapToTelemetrySessionViewModel(sessions));
        }

        private async Task<List<TelemetrySessionViewModel>> MapToTelemetrySessionViewModel(
            List<TelemetrySession> sessions, CancellationToken ct = default)
        {
            _logger.LogInformation("BEGGINING MAPPING");
            var viewModels = new List<TelemetrySessionViewModel>();
            foreach (var session in sessions)
            {
                var user = await _users.Find(u => u.Id == session.UserId.ToString()).FirstOrDefaultAsync(ct);
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                var filter = Builders<Page>.Filter.In(x => x.Id, session.PagesIds.Select(ObjectId.Parse).ToList());

                //var pages = await _pages.Find(p => session.PagesIds.Contains(p.Id.ToString())).ToListAsync(ct);
                var pages = await _pages.Find(filter).ToListAsync(ct);

                var pagesViewModels = pages.Select(p => new UserPageViewModel
                {
                    Page = new PageViewModel
                    {
                        Id = p.Id.ToString(),
                        Title = p.Title
                    },
                    // to zly czas chyba
                    Time = session.Time
                }).ToList();

                var viewModel = new TelemetrySessionViewModel
                {
                    Id = session.Id.ToString(),
                    SessionDate = session.SessionDate,
                    Status = session.Status,
                    Time = session.Time,
                    UserId = session.UserId,
                    User = userViewModel,
                    Pages = pagesViewModels
                };
                viewModels.Add(viewModel);
            }

            _logger.LogInformation("END MAPPING");
            return viewModels;
        }

        //// GET: TelemetrySessions/Details/5
        public async Task<IActionResult> Details(string? id, CancellationToken ct = default)
        {
            if (id is null)
                return NotFound();

            var filter = Builders<TelemetrySession>.Filter.Where(x => x.Id == ObjectId.Parse(id));

            //var telemetrySession = await _telemetrySessions
            //    .Find(t => t.Id.ToString() == id)
            //    .FirstOrDefaultAsync(ct);
            var telemetrySession = await _telemetrySessions.Find(filter).FirstOrDefaultAsync(ct);
            if (telemetrySession is null)
            {
                return NotFound();
            }

            return View(await MapToViewModel(telemetrySession));
        }

        private async Task<TelemetrySessionViewModel> MapToViewModel(TelemetrySession session,
            CancellationToken ct = default)
        {
            var user = await _users.Find(u => u.Id == session.UserId).FirstOrDefaultAsync(ct);
            var filter = Builders<Page>.Filter.In(p => p.Id, session.PagesIds.Select(ObjectId.Parse).ToList());
            //var pages = await _pages.Find(p => session.PagesIds.Contains(p.Id.ToString())).ToListAsync(ct);
            var pages = await _pages.Find(filter).ToListAsync(ct);
            var viewmodel = new TelemetrySessionViewModel
            {
                Id = session.Id.ToString(),
                UserId = session.UserId,
                SessionDate = session.SessionDate,
                Status = session.Status,
                Time = session.Time,
                User = new UserViewModel
                {
                    Email = user.Email,
                    Id = user.Id
                },
                Pages = pages.Select(p => new UserPageViewModel
                {
                    Page = new PageViewModel
                    {
                        Id = p.Id.ToString(),
                        Title = p.Title
                    },
                    Time = session.Time
                }).ToList()
            };

            return viewmodel;
        }

        //// GET: TelemetrySessions/Delete/5
        public async Task<IActionResult> Delete(string? id, CancellationToken ct = default)
        {
            if (id is null)
                return NotFound();

            var filter = Builders<TelemetrySession>.Filter.Where(s => s.Id == ObjectId.Parse(id));

            //var telemetrySession = await _telemetrySessions.Find(s => s.Id.ToString() == id).FirstOrDefaultAsync(ct);
            var telemetrySession = await _telemetrySessions.Find(filter).FirstOrDefaultAsync(ct);
            if (telemetrySession is null)
            {
                return NotFound();
            }

            return View(await MapToViewModel(telemetrySession, ct));
        }

        //// POST: TelemetrySessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken ct = default)
        {
            await _telemetrySessions
                .DeleteOneAsync(s => s.Id.ToString() == id, cancellationToken: ct);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TelemetrySessionExists(string id, CancellationToken ct = default)
        {
            var session = await _telemetrySessions
                .Find(s => s.Id.ToString() == id)
                .FirstOrDefaultAsync(ct);

            return session is not null;
        }

        [Authorize]
        public async Task<IActionResult> CreateNewSession(CancellationToken ct = default)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            TelemetrySession session = new TelemetrySession
            {
                UserId = currentUser.Id,
                Status = 1,
                Time = 0,
                SessionDate = DateTime.UtcNow
            };

            await _telemetrySessions.InsertOneAsync(session, cancellationToken: ct);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangeSessionStatus(string id, CancellationToken ct = default)
        {
            var updateDefinition = Builders<TelemetrySession>.Update
                .Set(t => t.Status, 0);

            await _telemetrySessions.UpdateOneAsync(s => s.UserId == id && s.Status == 1, updateDefinition,
                cancellationToken: ct);
            return RedirectToAction("Index", "Home");
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> SendInformationToSession([FromBody] Object data,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Inside SendInformationToSession");
            var data1 = JsonConvert.DeserializeObject<dynamic>(data.ToString());
            string pageName = Convert.ToString(data1.tabTitle);
            double seconds = Convert.ToDouble(data1.timeSpent);
            seconds /= 1000;
            var currentUser = await _userManager.GetUserAsync(User);
            var session = await _telemetrySessions.Find(t => t.Status == 1 && t.UserId == currentUser.Id)
                .FirstOrDefaultAsync(ct);

            var pageExists = (await _pages.Find(p => p.Title == pageName).FirstOrDefaultAsync(ct)) is not null;

            _logger.LogInformation("After PageExists");

            Page page;
            if (pageExists)
                page = await _pages.Find(p => p.Title == pageName).FirstOrDefaultAsync(ct);
            else
            {
                page = new Page()
                {
                    Title = pageName
                };

                await _pages.InsertOneAsync(page, cancellationToken: ct);
            }

            UserPage userPage;

            _logger.LogInformation("Before ASDF");
            var asdf = await _pages.Find(p => p.Title == pageName).FirstOrDefaultAsync(ct);
            _logger.LogInformation("Before LONG CONDITION");
            var condition =
                await _userPages
                    .Find(p => p.PageId == asdf.Id.ToString() && p.UserId == currentUser.Id &&
                               p.TelemetrySessionId == session.Id.ToString()).FirstOrDefaultAsync(ct) is not null;

            _logger.LogInformation("After LONG CONDITION");

            if (condition)
            {
                var updateDefinition = Builders<UserPage>.Update.Inc(x => x.Time, seconds);

                _logger.LogInformation("Updating LONG CONDITION");

                var filter = Builders<UserPage>.Filter.Where(x =>
                    x.UserId == currentUser.Id && x.PageId == asdf.Id.ToString() &&
                    x.TelemetrySessionId == session.Id.ToString());

                await _userPages.UpdateOneAsync(filter, updateDefinition,
                    cancellationToken: ct);

                _logger.LogInformation("AFTER Updating LONG CONDITION");
            }
            else
            {
                _logger.LogInformation("BEFORE ELSE USERPAGE");
                userPage = new UserPage()
                {
                    UserId = currentUser.Id,
                    PageId = page.Id.ToString(),
                    TelemetrySessionId = session.Id.ToString(),
                    Time = seconds
                };

                _logger.LogInformation("AFTER ELSE USERPAGE");

                await _userPages.InsertOneAsync(userPage, cancellationToken: ct);

                _logger.LogInformation("AFTER INSERT ELSE USERPAGE");
            }

            var sessionUpdateDefinition = Builders<TelemetrySession>.Update.Inc(x => x.Time, seconds);

            await _telemetrySessions.UpdateOneAsync(x => x.Id == session.Id,
                sessionUpdateDefinition, cancellationToken: ct);

            _logger.LogInformation("Before OK");

            return Ok();
        }
    }
}