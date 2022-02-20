using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Timetable.Controllers
{
    public class TeacherController : Controller
    {
        private TeacherContext db;

        public TeacherController(TeacherContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Teacher(TeacherSortState sortOrder = TeacherSortState.FIOAsc)
        {
            IQueryable<Teacher> teacher = db.Teachers;

            ViewData["FIOSort"] = sortOrder == TeacherSortState.FIOAsc ? TeacherSortState.FIODesc : TeacherSortState.FIOAsc;
            ViewData["SpecializationSort"] = sortOrder == TeacherSortState.SpecializationAsc ? TeacherSortState.SpecializationDesc : TeacherSortState.SpecializationAsc;
            ViewData["DepartmentSort"] = sortOrder == TeacherSortState.DepartmentAsc ? TeacherSortState.DepartmentDesc : TeacherSortState.DepartmentAsc;
            ViewData["TitleSort"] = sortOrder == TeacherSortState.TitleAsc ? TeacherSortState.TitleDesc : TeacherSortState.TitleAsc;

            teacher = sortOrder switch
            {
                TeacherSortState.FIODesc => teacher.OrderByDescending(s => s.FIO),
                TeacherSortState.SpecializationAsc => teacher.OrderBy(s => s.Specialization),
                TeacherSortState.SpecializationDesc => teacher.OrderByDescending(s => s.Specialization),
                TeacherSortState.DepartmentAsc => teacher.OrderBy(s => s.Department),
                TeacherSortState.DepartmentDesc => teacher.OrderByDescending(s => s.Department),
                TeacherSortState.TitleAsc => teacher.OrderBy(s => s.Title),
                TeacherSortState.TitleDesc => teacher.OrderByDescending(s => s.Title),
                _ => teacher.OrderBy(s => s.FIO),
            };
            return View(await teacher.AsNoTracking().ToListAsync());
        }

        public IActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(Teacher teacher)
        {
            db.Teachers.Add(teacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        public async Task<IActionResult> DetailsTeacher(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await db.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        public async Task<IActionResult> EditTeacher(int? id)
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
        public async Task<IActionResult> EditTeacher(Teacher teacher)
        {
            db.Teachers.Update(teacher);
            await db.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        [HttpGet]
        [ActionName("DeleteTeacher")]
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
        public async Task<IActionResult> DeleteTeacher(int? id)
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


    }
}
