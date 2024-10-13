using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Setup.Data;
using Setup.Hubs;
using System.Security.Claims;

namespace Setup.Controllers
{
    [Authorize]
    [Controller]
    public class DammenController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly WebpageDBContext _context;

        public DammenController(IHubContext<GameHub> hubContext, WebpageDBContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        // GET: DammenController
        public ActionResult Index()
        {
            var damSpellen = _context.DamSpel?.ToList();
            var spelers = _context.Speler?.ToList();
            ViewBag.Spelers = spelers;
            ViewBag.DamSpellen = damSpellen;
            return View();
        }
        public ActionResult Spel(int id)
        {
            using (_context)
            {
                DamSpel? damSpel = _context.DamSpel?.Find(id);
                if (damSpel != null)
                {
                    string[] spelers = { damSpel.Creator, (damSpel.Deelnemer != null ? damSpel.Deelnemer : "") };
                    var gebruiker = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ViewBag.Spelers = JsonConvert.SerializeObject(spelers);
                    ViewBag.Gebruiker = gebruiker;
                    ViewBag.BordStand = damSpel.BordStand;  // Pass the BordStand value to the view
                    ViewBag.Id = damSpel.Id;
                    ViewBag.AanZet = damSpel.AanZet;
                    return View(damSpel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }


        // GET: DammenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DammenController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DammenController/Create
        [HttpPost]
        public async Task<IActionResult> Create(DamSpel model)
        {
            using (_context)
            {
                DamBord bord = new DamBord(0);
                DatabaseSaving(bord, _context, "Add");
                //correcte opzet
                //DamSpel spel = new DamSpel(0, model.SpelNaam, null, User.FindFirstValue(ClaimTypes.NameIdentifier), null, bord.Id, false, "0101010110101010010101010000000000000000202020200202020220202020");
                //test opzet
                DamSpel spel = new DamSpel(0, model.SpelNaam, null, User.FindFirstValue(ClaimTypes.NameIdentifier), null, bord.Id, false, "0101010110101010010101010000000002020202000000000202020200000000", User.FindFirstValue(ClaimTypes.NameIdentifier));
                DatabaseSaving(spel, _context, "Add");
                await _hubContext.Clients.All.SendAsync("GameListChanged");

            }
            return RedirectToAction("Index");
        }

        // GET: DammenController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DammenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete(DamSpel? spel)
        {
            using (_context)
            {
                DamSpel? damSpel = _context.DamSpel?.Find(spel.Id);
                if (damSpel == null)
                {
                    return Json(new { success = false, message = "Spel kon niet worden gevonden." });
                }
                DatabaseSaving(damSpel, _context, "Remove");
            }
            return Json(new { success = true });
        }
        private void DatabaseSaving(object obj, WebpageDBContext context, string type)
        {
            switch (type)
            {
                case "Add":
                    context.Add(obj);
                    context.SaveChanges();
                    break;
                case "Update":
                    context.Update(obj);
                    context.SaveChanges();
                    break;
                case "Remove":
                    context.Remove(obj);
                    context.SaveChanges();
                    break;
            }
        }
        [HttpPost]
        public IActionResult AddPlayerToGame(GameData gameData)
        {
            var speler1 = gameData?.Speler1;
            var speler2 = gameData?.Speler2;
            var spel = gameData?.DamSpel;

            if (speler1 != null && speler2 != null && spel != null)
            {
                if (speler1.ToString().Equals(speler2.ToString()))
                {
                    return Json(new { success = false, message = "Je kan niet je eigen game joinen!" });
                }
                if (speler1 != null && spel.Deelnemer != null)
                {
                    if (speler1 == speler2 || spel.Deelnemer == speler2)
                    {
                        return Json(new { success = false, message = "Je zit al in deze game!" });
                    }
                    return Json(new { success = false, message = "Deze game zit al vol!" });
                }
                using (_context)
                {
                    DamSpel? damSpel = _context.DamSpel?.Find(spel.Id);
                    if (damSpel != null)
                    {
                        damSpel.Deelnemer = speler2;
                        DatabaseSaving(damSpel, _context, "Update");
                        return Json(new { success = true, id = spel.Id });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Dit spel kon niet gevonden worden." });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Oeps! Er ging iets mis!" });
            }
        }
        [HttpGet]
        public IActionResult GetGameLijst()
        {
            var damSpellen = _context.DamSpel?.OrderBy(e => e.Id).ToList();
            var spelers = _context.Speler?.ToList();
            var gebruiker = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lijstData = new Dictionary<string, object?>
        {
            { "Spellen", damSpellen },
            { "Spelers",  spelers },
            { "Gebruiker", gebruiker}
        };
            return Json(lijstData);
        }
        [HttpPost]
        public IActionResult UpdateBoardData(string gameState, string gameId, string beurt)
        {
            using (_context)
            {
                DamSpel? spel = _context.DamSpel?.Find(Int32.Parse(gameId));
                if (spel == null)
                {
                    return Json(new { success = false, message = "Spel kon niet worden gevonden." });
                }
                else
                {
                    spel.BordStand = gameState;
                    spel.AanZet = beurt;
                    DatabaseSaving(spel, _context, "Update");
                    return Json(new { success = true });
                }
            }
        }
    }
    public class GameData
    {
        //Player object
        public string? Speler1 { get; set; }
        //Player email identifier
        public string? Speler2 { get; set; }
        //Game identifier
        public DamSpel? DamSpel { get; set; }
    }
}
