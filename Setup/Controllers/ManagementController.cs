using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Setup.Controllers
{
    public class ManagementController : Controller
    {
        // GET: ManagementController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ManagementController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManagementController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagementController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: ManagementController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ManagementController/Edit/5
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

        // GET: ManagementController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManagementController/Delete/5
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
