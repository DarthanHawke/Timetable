using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Timetable.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        private TeacherContext tdb;

        public TeacherController(TeacherContext context)
        {
            tdb = context;
        }

        public async Task<IActionResult> Teacher(TeacherSortState sortOrder = TeacherSortState.FIOAsc)
        {
            IQueryable<Teacher> teacher = tdb.Teachers;

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
            tdb.Teachers.Add(teacher);
            await tdb.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        public async Task<IActionResult> DetailsTeacher(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await tdb.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        public async Task<IActionResult> EditTeacher(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await tdb.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
                if (teacher != null)
                    return View(teacher);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditTeacher(Teacher teacher)
        {
            tdb.Teachers.Update(teacher);
            await tdb.SaveChangesAsync();
            return RedirectToAction("Teacher");
        }

        [HttpGet]
        [ActionName("DeleteTeacher")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Teacher teacher = await tdb.Teachers.FirstOrDefaultAsync(p => p.Id_Teacher == id);
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
                tdb.Entry(teacher).State = EntityState.Deleted;
                await tdb.SaveChangesAsync();
                return RedirectToAction("Teacher");
            }
            return NotFound();
        }
    }
}
