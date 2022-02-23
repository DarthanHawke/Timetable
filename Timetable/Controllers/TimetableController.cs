using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Timetable.Controllers
{
    [Authorize]
    public class TimetableController : Controller
    {
        private TimetableContext ttdb;

        public TimetableController(TimetableContext context)
        {
            ttdb = context;
        }

        public IQueryable<NewTimetable> NewTimetable()
        {
            IQueryable<NewTimetable> NewTimetables = from timetable in ttdb.Timetables
                                                     join groups in ttdb.Groups on timetable.Id_Group equals groups.Id_Group
                                                     join teachers in ttdb.Teachers on timetable.Id_Teacher equals teachers.Id_Teacher
                                                     join lessons in ttdb.Lessons on timetable.Id_Lesson equals lessons.Id_Lesson
                                                     join classrooms in ttdb.Classrooms on timetable.Id_Class equals classrooms.Id_Class
                                                     select new NewTimetable
                                                     {
                                                         Date = timetable.Date,
                                                         Course = groups.Course,
                                                         NumberGroup = groups.NumberGroup,
                                                         FIOteacher = teachers.FIO,
                                                         NameLesson = lessons.Name,
                                                         NumberClass = classrooms.NumberClass
                                                     };
            return NewTimetables;
        }

        public IActionResult Timetable()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Timetable(СhoiceViewModel model)
        {
            string[] weekday = new string[6] { "2022-02-21T", "2022-02-22T", "2022-02-23T", "2022-02-24T", "2022-02-25T", "2022-02-26T" };
            string[] time = new string[8] { "08:00:00", "09:45:00", "11:30:00", "13:25:00", "15:10:00", "16:55:00", "18:40:00", "20:10:00" };


            var courseset = model.ViewCourses(model.PeriodCourses);
            var groupeset = model.ViewGroups(model.PeriodGroups);


            var timetable = ttdb.Groups.Where(p => p.Course.ToString() == courseset)
                .Where(p => p.NumberGroup.ToString() == groupeset).Select(p => p.Id_Group).ToArray()[0];

            var pairtb = ttdb.Timetables.Where(p => p.Id_Group == timetable);


            Teacher[,] teachers = new Teacher[6, 8];
            LessonU[,] lessons = new LessonU[6, 8];
            Classroom[,] classrooms = new Classroom[6, 8];
            CultureInfo provider = CultureInfo.InvariantCulture;

            for (var i = 0; i < 6; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    DateTime pairdate = DateTime.ParseExact(weekday[i] + time[j], "yyyy-MM-ddTHH:mm:ss", provider);
                    var pair = pairtb.FirstOrDefault(p => p.Date == pairdate);
                    if (pair != null)
                    {
                        teachers[i, j] = ttdb.Teachers.FirstOrDefault(p => p.Id_Teacher == pair.Id_Teacher);
                        lessons[i, j] = ttdb.Lessons.FirstOrDefault(p => p.Id_Lesson == pair.Id_Lesson);
                        classrooms[i, j] = ttdb.Classrooms.FirstOrDefault(p => p.Id_Class == pair.Id_Class);
                    }
                    else
                    {
                        teachers[i, j] = new Teacher { FIO = "" };
                        lessons[i, j] = new LessonU { Name = "" };
                        classrooms[i, j] = new Classroom { NumberClass = "" };
                    }
                }
            }

            ViewBag.teachers = new string[6,8];
            ViewBag.lessons = new string[6, 8];
            ViewBag.classrooms = new string[6, 8];


            for (var i = 0; i < 6; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    ViewBag.teachers[i, j] = teachers[i, j].FIO;
                    ViewBag.lessons[i, j] = lessons[i, j].Name;
                    ViewBag.classrooms[i, j] = classrooms[i, j].NumberClass;
                }
            }
            ViewBag.courseset = model.ViewCourses(model.PeriodCourses);
            ViewBag.groupeset = model.ViewGroups(model.PeriodGroups);
            return View("СhoiceTimetable");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> СhangeTimetable(TimetableSortState sortOrder = TimetableSortState.DateAsc)
        {
            IQueryable<NewTimetable> newtimetables = NewTimetable();

            ViewData["DateSort"] = sortOrder == TimetableSortState.DateAsc ? TimetableSortState.DateDesc : TimetableSortState.DateAsc;
            ViewData["CourseSort"] = sortOrder == TimetableSortState.CourseAsc ? TimetableSortState.CourseDesc : TimetableSortState.CourseAsc;
            ViewData["NumberGroupSort"] = sortOrder == TimetableSortState.NumberGroupAsc ? TimetableSortState.NumberGroupDesc : TimetableSortState.NumberGroupAsc;
            ViewData["FIOteacherSort"] = sortOrder == TimetableSortState.FIOteacherAsc ? TimetableSortState.FIOteacherDesc : TimetableSortState.FIOteacherAsc;
            ViewData["NameLessonSort"] = sortOrder == TimetableSortState.NameLessonAsc ? TimetableSortState.NameLessonDesc : TimetableSortState.NameLessonAsc;
            ViewData["NumberClassSort"] = sortOrder == TimetableSortState.NumberClassAsc ? TimetableSortState.NumberClassDesc : TimetableSortState.NumberClassAsc;

            newtimetables = sortOrder switch
            {
                TimetableSortState.DateDesc => newtimetables.OrderByDescending(s => s.Date),
                TimetableSortState.CourseAsc => newtimetables.OrderBy(s => s.Course),
                TimetableSortState.CourseDesc => newtimetables.OrderByDescending(s => s.Course),
                TimetableSortState.NumberGroupAsc => newtimetables.OrderBy(s => s.NumberGroup),
                TimetableSortState.NumberGroupDesc => newtimetables.OrderByDescending(s => s.NumberGroup),
                TimetableSortState.FIOteacherAsc => newtimetables.OrderBy(s => s.FIOteacher),
                TimetableSortState.FIOteacherDesc => newtimetables.OrderByDescending(s => s.FIOteacher),
                TimetableSortState.NameLessonAsc => newtimetables.OrderBy(s => s.NameLesson),
                TimetableSortState.NameLessonDesc => newtimetables.OrderByDescending(s => s.NameLesson),
                TimetableSortState.NumberClassAsc => newtimetables.OrderBy(s => s.NumberClass),
                TimetableSortState.NumberClassDesc => newtimetables.OrderByDescending(s => s.NumberClass),
                _ => newtimetables.OrderBy(s => s.Date),
            };

            return View(await newtimetables.AsNoTracking().ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTimetable(TimetableU timetable)
        {
            ttdb.Timetables.Add(timetable);
            await ttdb.SaveChangesAsync();
            return RedirectToAction("СhangeTimetable");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsTimetable(NewTimetable newtimetables)
        {
            int? id = newtimetables.Id_Date;
            if (id != null)
            {
                TimetableU timetable = await ttdb.Timetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (timetable != null)
                    return View(timetable);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTimetable(int? id)
        {
            if (id != null)
            {
                TimetableU timetable = await ttdb.Timetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (timetable != null)
                    return View(timetable);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTimetable(TimetableU timetable)
        {
            ttdb.Timetables.Update(timetable);
            await ttdb.SaveChangesAsync();
            return RedirectToAction("СhangeTimetable");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteTimetable")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                TimetableU timetable = await ttdb.Timetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (timetable != null)
                    return View(timetable);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTimetable(int? id)
        {
            if (id != null)
            {
                TimetableU timetable = new TimetableU { Id_Date = id.Value };
                ttdb.Entry(timetable).State = EntityState.Deleted;
                await ttdb.SaveChangesAsync();
                return RedirectToAction("СhangeTimetable");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchString)
        {
            IQueryable<NewTimetable> newtimetables = NewTimetable();

            IQueryable<NewTimetable> allsearch = newtimetables.Where(a => a.NumberClass.Contains(searchString))
            .Union(newtimetables.Where(a => a.NumberGroup.ToString().Contains(searchString)))
            .Union(newtimetables.Where(a => a.Course.ToString().Contains(searchString)))
            .Union(newtimetables.Where(a => a.NameLesson.Contains(searchString)))
            .Union(newtimetables.Where(a => a.FIOteacher.Contains(searchString)));
            if (allsearch == null || !allsearch.Any())
            {
                ViewBag.Error = "Упс. Похоже по вашему запросу ничего не нашлось.";
                return NotFound();
            }
            return View(await allsearch.ToListAsync());
        }
    }
}
