using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Setup.Data;
using Setup.Hubs;
using System.Security.Claims;

namespace Setup.Controllers
{
    [Controller]
    public class DammenController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;

        public DammenController(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // GET: DammenController
        public ActionResult Index()
        {
            var dbContext = new WebpageDBContext();
            var damSpellen = dbContext.DamSpel?.ToList();
            var spelers = dbContext.Speler?.ToList();
            ViewBag.Spelers = spelers;
            ViewBag.DamSpellen = damSpellen;
            return View();
        }
        public ActionResult Spel(int id)
        {
            using (var context = new WebpageDBContext())
            {
                DamSpel? damSpel = context.DamSpel?.Find(id);
                if (damSpel != null)
                {
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
            using (var dbContext = new WebpageDBContext())
            {
                DamBord bord = new DamBord(0);
                DatabaseSaving(bord, dbContext, "Add");
                DamSpel spel = new DamSpel(0, model.SpelNaam, null, User.FindFirstValue(ClaimTypes.NameIdentifier), null, bord.Id, false);
                DatabaseSaving(spel, dbContext, "Add");
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
        public IActionResult Delete( DamSpel? spel)
        {
            using (var dbContext = new WebpageDBContext())
            {
                DamSpel? damSpel = dbContext.DamSpel?.Find(spel.Id);
                if (damSpel == null)
                {
                    return Json(new { success = false, message = "Spel kon niet worden gevonden." });
                }
                DatabaseSaving(damSpel, dbContext, "Remove");
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
                using (var context = new WebpageDBContext())
                {
                    DamSpel? damSpel = context.DamSpel?.Find(spel.Id);
                    if (damSpel != null)
                    {
                        damSpel.Deelnemer = speler2;
                        DatabaseSaving(damSpel, context, "Update");
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
            var dbContext = new WebpageDBContext();
            var damSpellen = dbContext.DamSpel?.OrderBy(e => e.Id).ToList();
            var spelers = dbContext.Speler?.ToList();
            var gebruiker = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lijstData = new Dictionary<string, object?>
        {
            { "Spellen", damSpellen },
            { "Spelers",  spelers },
            { "Gebruiker", gebruiker}
        };
            return Json(lijstData);
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
