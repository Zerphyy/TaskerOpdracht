using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Setup.Data;
using Setup.Hubs;
using System.Security.Claims;

namespace Setup.Controllers
{
    [Authorize]
    public class ManagementController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly WebpageDBContext _context;
        public ManagementController(IHubContext<GameHub> hubContext, WebpageDBContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        public ActionResult Index()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (CheckUserRole(userEmail))
            {
                HttpContext.Session.SetString("UserEmail", userEmail);
                ViewBag.User = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return View();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }


        [HttpPost]
        public IActionResult PromoteUser(string userId)
        {
            var user = _context.Speler?.Find(userId);
            if (user != null)
            {
                user.Rol = "Moderator";
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not found" });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = _context.Speler?.Find(userId);
            if (user != null)
            {
                _context.Speler.Remove(user);
                _context.SaveChanges();

                await _hubContext.Clients.User(userId).SendAsync("Removed");

                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not found" });
        }
        public IActionResult GetUserLijst()
        {
            List<Gebruiker> gebruikerLijst = null;
            var gebruikers = _context.Speler?.ToList();
            if (gebruikers?.Count() > 0)
            {
                gebruikerLijst = gebruikers;
            }
            return Json(gebruikerLijst);
        }
        public IActionResult GetUserStats(Speler user)
        {
            var userStats = _context.SpelerStats?.FirstOrDefault(s => s.Speler == user.Email);
            if (userStats != null)
            {
                return Json(userStats);
            }
            return Json(null);
        }
        public IActionResult GetUserRole(string user)
        {
            var User = _context.Speler?.FirstOrDefault(s => s.Email == user);
            if (User != null)
            {
                return Json(User.Rol);
            }
            return Json(null);
        }
        private bool CheckUserRole(string userId)
        {
            if (_context.Speler?.Find(userId).Rol == "Moderator" || _context.Speler?.Find(userId).Rol == "Admin")
            {
                return true;
            }
            return false;
        }

    }
}
