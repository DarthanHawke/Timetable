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

        [HttpPost]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            db.Teachers.Add(teacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            db.Teachers.Update(teacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Teacher teacher = new Teacher { Id_Teacher = id.Value };
                db.Entry(teacher).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Teacher");
            }
            return NotFound();
        }

        public IActionResult index()
        {
            return View();
        }
    }
}
