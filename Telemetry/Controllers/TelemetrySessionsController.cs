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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telemetry.Entities;
using Telemetry.Entities.Models;

namespace Telemetry.Controllers
{
    public class TelemetrySessionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TelemetrySessionsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TelemetrySessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TelemetrySessions.Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TelemetrySessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TelemetrySessions == null)
            {
                return NotFound();
            }

            var telemetrySession = await _context.TelemetrySessions
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telemetrySession == null)
            {
                return NotFound();
            }

            return View(telemetrySession);
        }

        // GET: TelemetrySessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TelemetrySessions == null)
            {
                return NotFound();
            }

            var telemetrySession = await _context.TelemetrySessions
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telemetrySession == null)
            {
                return NotFound();
            }

            return View(telemetrySession);
        }

        // POST: TelemetrySessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TelemetrySessions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TelemetrySessions'  is null.");
            }
            var telemetrySession = await _context.TelemetrySessions.FindAsync(id);
            if (telemetrySession != null)
            {
                _context.TelemetrySessions.Remove(telemetrySession);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelemetrySessionExists(int id)
        {
          return _context.TelemetrySessions.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> CreateNewSession()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            TelemetrySession session = new TelemetrySession()
            {
                UserId = currentUser.Id,
                Status = 1,
                Time = 0,
                SessionDate = DateTime.UtcNow
            };

            await _context.AddAsync(session);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangeSessionStatus(string id)
        {
            var session = await _context.TelemetrySessions.FirstAsync(x => x.UserId == id && x.Status == 1);
            session.Status = 0;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendInformationToSession([FromBody]Object data)
        {
            var data1 = JsonConvert.DeserializeObject<dynamic>(data.ToString());
            string pageName = Convert.ToString(data1.tabTitle);
            double seconds = Convert.ToDouble(data1.timeSpent);
            seconds /= 1000;
            var currentUser = await _userManager.GetUserAsync(User);
            TelemetrySession session = await _context.TelemetrySessions.FirstAsync(x => x.Status == 1 && x.UserId == currentUser.Id);
            bool pageExist = await _context.Pages.AnyAsync(x => x.Title == pageName);

            Page page;
            if (pageExist == true)
                page = await _context.Pages.FirstAsync(x => x.Title == pageName);
            else
            {
                page = new Page()
                {
                    Title = pageName
                };
                await _context.AddAsync(page);
            }
            await _context.SaveChangesAsync();

            UserPage userPage;
            if (await _context.UserPages.AnyAsync(x => x.Page.Title == pageName && x.User == currentUser && x.TelemetrySessionId == session.Id))
            {
                userPage = await _context.UserPages.FirstAsync(x => x.User == currentUser && 
                                                            x.Page.Title == pageName && 
                                                            x.TelemetrySessionId == session.Id);
                userPage.Time += seconds;
            }
            else
            {
                userPage = new UserPage()
                {
                    UserId = currentUser.Id,
                    PageId = page.Id,
                    TelemetrySessionId = session.Id,
                    Time = seconds
                };
                await _context.AddAsync(userPage);
            }
            await _context.SaveChangesAsync();

            session.Time += seconds;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
