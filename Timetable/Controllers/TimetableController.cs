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
                                                         Week = timetable.Week,
                                                         Time = timetable.Time,
                                                         Integrity = timetable.Integrity,
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
            string[] weekday = new string[6] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            string[] time = new string[8] { "08:00", "09:45", "11:30", "13:25", "15:10", "16:55", "18:40", "20:10" };

            IQueryable<NewTimetable> newtimetables = NewTimetable();

            var courseset = model.ViewCourses(model.PeriodCourses);
            ViewBag.courseset = courseset;
            var groupeset = model.ViewGroups(model.PeriodGroups);
            ViewBag.groupeset = groupeset;

            newtimetables = newtimetables.Where(p => p.Course.ToString() == courseset)
                .Where(p => p.NumberGroup.ToString() == groupeset);

            IQueryable <NewTimetable> pairday = null;
            IQueryable<NewTimetable> pairtime = null;
            NewTimetable[,,] pair = new NewTimetable[6,8,3];

            for (var i = 0; i < 6; ++i)
            {
                pairday = newtimetables.Where(p => p.Week == weekday[i]);
                for (var j = 0; j < 8; ++j)
                {
                    pairtime = pairday.Where(p => p.Time == time[j]);
                    pair[i, j, 1] = pairtime.FirstOrDefault(p => p.Integrity == "Числитель");
                    pair[i, j, 2] = pairtime.FirstOrDefault(p => p.Integrity == "Знаменатель");
                    if (pair[i, j, 1] == null && pair[i, j, 2] == null)
                    {
                        pair[i, j, 0] = pairtime.FirstOrDefault();
                        if (pair[i, j, 0] == null)
                        {
                            pair[i, j, 0] = new NewTimetable
                            { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                        }
                        pair[i, j, 1] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                        pair[i, j, 2] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                    }
                    else if (pair[i, j, 1] == null)
                    {
                        pair[i, j, 0] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                        pair[i, j, 1] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                    }
                    else if (pair[i, j, 2] == null)
                    {
                        pair[i, j, 0] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                        pair[i, j, 2] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                    }
                    else if (pair[i, j, 0] == null)
                    {
                        pair[i, j, 0] = new NewTimetable
                        { Week = " ", Time = " ", Integrity = " ", Course = 0, NumberGroup = 0, FIOteacher = " ", NameLesson = " ", NumberClass = " " };
                    }
                }
            }

            ViewBag.weekday = weekday;
            ViewBag.time = time;
            ViewBag.pair = pair;

            return View("СhoiceTimetable");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> СhangeTimetable(TimetableSortState sortOrder = TimetableSortState.WeekAsc)
        {
            IQueryable<NewTimetable> newtimetables = NewTimetable();

            ViewData["WeekSort"] = sortOrder == TimetableSortState.WeekAsc ? TimetableSortState.WeekDesc : TimetableSortState.WeekAsc;
            ViewData["DateSort"] = sortOrder == TimetableSortState.TimeAsc ? TimetableSortState.TimeDesc : TimetableSortState.TimeAsc;
            ViewData["CourseSort"] = sortOrder == TimetableSortState.CourseAsc ? TimetableSortState.CourseDesc : TimetableSortState.CourseAsc;
            ViewData["NumberGroupSort"] = sortOrder == TimetableSortState.NumberGroupAsc ? TimetableSortState.NumberGroupDesc : TimetableSortState.NumberGroupAsc;
            ViewData["FIOteacherSort"] = sortOrder == TimetableSortState.FIOteacherAsc ? TimetableSortState.FIOteacherDesc : TimetableSortState.FIOteacherAsc;
            ViewData["NameLessonSort"] = sortOrder == TimetableSortState.NameLessonAsc ? TimetableSortState.NameLessonDesc : TimetableSortState.NameLessonAsc;
            ViewData["NumberClassSort"] = sortOrder == TimetableSortState.NumberClassAsc ? TimetableSortState.NumberClassDesc : TimetableSortState.NumberClassAsc;

            newtimetables = sortOrder switch
            {
                TimetableSortState.WeekDesc => newtimetables.OrderByDescending(s => s.Week),
                TimetableSortState.TimeAsc => newtimetables.OrderBy(s => s.Time),
                TimetableSortState.TimeDesc => newtimetables.OrderByDescending(s => s.Time),
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
                _ => newtimetables.OrderBy(s => s.Week),
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
