using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Setup.Data;

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
                dbContext.Add(new DamSpel(0, model.SpelNaam, null, 1, null, 1, false));
                dbContext.Add(new Speler(0, "Kevin", "bestmail@email.com", "password124"));
                dbContext.Add(new DamBord(0));
                dbContext.SaveChanges();
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
    }
}
