using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;

namespace Timetable.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult AuthorizeIndex()
        {
            return RedirectToAction("Index");
        }

        private ApplicationContext db;

        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Teacher()
        {
            return View(await db.Teachers.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost][Authorize]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            db.Teachers.Add(teacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult index()
        {
            return View();
        }
    }
}
