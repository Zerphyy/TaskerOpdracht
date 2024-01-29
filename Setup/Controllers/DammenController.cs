using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Setup.Data;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Setup.Controllers
{
    public class DammenController : Controller
    {
        // GET: DammenController
        public ActionResult Index()
        {
            var dbContext = new WebpageDBContext();
            var damSpellen = dbContext.DamSpel.ToList();
            var spelers = dbContext.Speler.ToList();
            var damBordVakjes = dbContext.DamBordVakje.ToList();
            ViewBag.Spelers = spelers;
            ViewBag.DamSpellen = damSpellen;
            ViewBag.DamBordVakjes = damBordVakjes;
            return View();
        }
        public ActionResult Spel()
        {
            return View("View");
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
        public ActionResult Create(Data.DamSpel? model)
        {
            using (var dbContext = new WebpageDBContext())
            {
                DamBord bord = new DamBord(0);
                DatabaseSaving(bord, dbContext);
                DamSpel spel = new DamSpel(0, model.SpelNaam, null, User.FindFirstValue(ClaimTypes.NameIdentifier), null, bord.Id, false);
                DatabaseSaving(spel, dbContext);
                
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

        // GET: DammenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DammenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
        private void DatabaseSaving(object obj, WebpageDBContext context)
        {
                context.Add(obj);
                context.SaveChanges();
        }
    }
}
