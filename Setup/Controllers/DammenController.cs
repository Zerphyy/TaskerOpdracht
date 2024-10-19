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
        private static Dictionary<string, DateTime> userLastCreationTime = new Dictionary<string, DateTime>();


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
                DamSpel? damSpel = _context.DamSpel?.Find(id);
                if (damSpel != null)
                {
                    string[] spelers = { damSpel.Creator, (damSpel.Deelnemer != null ? damSpel.Deelnemer : "") };
                    var gebruiker = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ViewBag.Spelers = JsonConvert.SerializeObject(spelers);
                    ViewBag.Gebruiker = gebruiker;
                    ViewBag.BordStand = damSpel.BordStand;
                    ViewBag.Id = damSpel.Id;
                    ViewBag.AanZet = damSpel.AanZet;
                    return View(damSpel);
                }
                else
                {
                    return RedirectToAction("Index");
                }
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
            string userId = model.Creator == null ? User.FindFirstValue(ClaimTypes.NameIdentifier) : model.Creator;
            if (userLastCreationTime.ContainsKey(userId))
            {
                DateTime lastCreationTime = userLastCreationTime[userId];
                if ((DateTime.UtcNow - lastCreationTime).TotalMilliseconds < 300)
                {
                    return Json(new { success = false, message = "You can only create one game per second." });
                }
            }
            userLastCreationTime[userId] = DateTime.UtcNow;
            DamBord bord = new DamBord(0);
            DatabaseSaving(bord, _context, "Add");
            DamSpel spel = new DamSpel(0, model.SpelNaam, null, userId, null, bord.Id, false, "0101010110101010010101010000000000000000202020200202020220202020", userId);
            DatabaseSaving(spel, _context, "Add");
            if (model.Creator == null)
            {
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
                DamSpel? damSpel = _context.DamSpel?.Find(spel.Id);
                if (damSpel == null)
                {
                    return Json(new { success = false, message = "Spel kon niet worden gevonden." });
                }
                DatabaseSaving(damSpel, _context, "Remove");
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
        [HttpPost]
        public IActionResult ProcessWin(string gameId, string winner, string[] players, string caller)
        {
            if (caller != winner)
            {
                return Json(new { success = false, message = "Wrong player sent the call!" });
            } else
            {
                    DamSpel? spel = _context.DamSpel?.Find(Int32.Parse(gameId));
                    if (spel == null)
                    {
                        return Json(new { success = false, message = "Spel kon niet worden gevonden." });
                    }
                    else
                    {
                        DatabaseSaving(spel, _context, "Remove");
                    }
                    Gebruiker? spelerWinner = _context.Speler?.Find(winner);
                    Gebruiker? spelerLoser = _context.Speler?.Find(winner == players[0] ? players[1] : players[0]);
                    GebruikerStats? spelerStatsWinner = _context.SpelerStats?.Find(winner);
                    GebruikerStats? spelerStatsLoser = _context.SpelerStats?.Find(winner == players[0] ? players[1] : players[0]);

                    if (spelerStatsWinner != null && spelerWinner != null && winner == spelerWinner.Email)
                    {
                        spelerStatsWinner.AantalSpellen += 1;
                        spelerStatsWinner.AantalGewonnen += 1;
                        spelerStatsWinner.WinLossRatio = spelerStatsWinner.AantalVerloren != 0 ? (100 / (spelerStatsWinner.AantalGewonnen + spelerStatsWinner.AantalVerloren)) * spelerStatsWinner.AantalGewonnen : 100;
                        DatabaseSaving(spelerStatsWinner, _context, "Update");
                    }
                    if (spelerStatsLoser != null && spelerLoser != null && winner != spelerLoser.Email)
                    {
                        spelerStatsLoser.AantalSpellen += 1;
                        spelerStatsLoser.AantalVerloren += 1;
                        spelerStatsLoser.WinLossRatio = (100 / (spelerStatsLoser.AantalGewonnen + spelerStatsLoser.AantalVerloren)) * spelerStatsLoser.AantalGewonnen;
                        DatabaseSaving(spelerStatsLoser, _context, "Update");
                    }

                    //create stats fields if either one or both playerstats dont exist
                    if (spelerStatsWinner == null && spelerWinner != null && spelerWinner.Email == winner)
                    {
                        GebruikerStats winnaar = new GebruikerStats
                        {
                            Speler = spelerWinner.Email,
                            AantalSpellen = 1,
                            AantalGewonnen = 1,
                            AantalVerloren = 0,
                            WinLossRatio = 100
                        };
                        DatabaseSaving(winnaar, _context, "Add");
                    }
                    if (spelerStatsLoser == null && spelerLoser != null && spelerLoser.Email != winner)
                    {
                        GebruikerStats loser = new GebruikerStats
                        {
                            Speler = spelerLoser.Email,
                            AantalSpellen = 1,
                            AantalGewonnen = 0,
                            AantalVerloren = 1,
                            WinLossRatio = 0
                        };
                        DatabaseSaving(loser, _context, "Add");
                    }

                    return Json(new { success = true });
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
