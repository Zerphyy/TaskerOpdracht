using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Setup.Data;

namespace Setup.Controllers
{
    public class DammenController : Controller
    {
        // GET: DammenController
        public ActionResult Index(Data.DamSpel? model)
        {
            return View();
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
                dbContext.DamSpel.Add(new DamSpel(0, "Hello World 2", null, null, 0, new Speler(0, "Kevin", "bestmail@email.com", "password124"),null, null,0, new DamBord(0), false));
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
