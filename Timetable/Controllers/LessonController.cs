using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Timetable.Controllers
{
    public class LessonController : Controller
    {
        private LessonContext ldb;

        public LessonController(LessonContext context)
        {
            ldb = context;
        }

        public async Task<IActionResult> Lesson(LessonSortState sortOrder = LessonSortState.NameAsc)
        {
            IQueryable<LessonU> lesson = ldb.Lessons;

            ViewData["TypeLessonSort"] = sortOrder == LessonSortState.TypeLessonAsc ? LessonSortState.TypeLessonDesc : LessonSortState.TypeLessonAsc;
            ViewData["NameSort"] = sortOrder == LessonSortState.NameAsc ? LessonSortState.NameDesc : LessonSortState.NameAsc;

            lesson = sortOrder switch
            {
                LessonSortState.NameDesc => lesson.OrderByDescending(s => s.Name),
                LessonSortState.TypeLessonAsc => lesson.OrderBy(s => s.TypeLesson),
                LessonSortState.TypeLessonDesc => lesson.OrderByDescending(s => s.TypeLesson),
                _ => lesson.OrderBy(s => s.Name),
            };
            return View(await lesson.AsNoTracking().ToListAsync());
        }

        public IActionResult CreateLesson()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson(LessonU lesson)
        {
            ldb.Lessons.Add(lesson);
            await ldb.SaveChangesAsync();
            return RedirectToAction("Lesson");
        }

        public async Task<IActionResult> DetailsLesson(int? id)
        {
            if (id != null)
            {
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        public async Task<IActionResult> EditLesson(int? id)
        {
            if (id != null)
            {
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditLesson(LessonU lesson)
        {
            ldb.Lessons.Update(lesson);
            await ldb.SaveChangesAsync();
            return RedirectToAction("Lesson");
        }

        [HttpGet]
        [ActionName("DeleteLesson")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLesson(int? id)
        {
            if (id != null)
            {
                LessonU lesson = new LessonU { Id_Lesson = id.Value };
                ldb.Entry(lesson).State = EntityState.Deleted;
                await ldb.SaveChangesAsync();
                return RedirectToAction("Lesson");
            }
            return NotFound();
        }
    }
}
