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

        public class Lists
        {
            public string Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Lists> weeks = new List<Lists>
        {
            new Lists { Id = "Понедельник", Names = "Понедельник" },
            new Lists { Id = "Вторник", Names = "Вторник" },
            new Lists { Id = "Среда", Names = "Среда" },
            new Lists { Id = "Четверг", Names = "Четверг" },
            new Lists { Id = "Пятница", Names = "Пятница" },
            new Lists { Id = "Суббота", Names = "Суббота" }
        };

        IEnumerable<Lists> times = new List<Lists>
        {
            new Lists { Id = "08:00", Names = "08:00" },
            new Lists { Id = "09:45", Names = "09:45" },
            new Lists { Id = "11:30", Names = "11:30" },
            new Lists { Id = "13:25", Names = "13:25" },
            new Lists { Id = "15:10", Names = "15:10" },
            new Lists { Id = "16:55", Names = "16:55" },
            new Lists { Id = "18:40", Names = "18:40" },
            new Lists { Id = "20:10", Names = "20:10" }
        };

        IEnumerable<Lists> integrits = new List<Lists>
        {
            new Lists { Id = "Числитель", Names = "Числитель" },
            new Lists { Id = "Знаменатель", Names = "Знаменатель" },
            new Lists { Id = "Целое", Names = "Целое" }
        };

        public class Listsint
        {
            public int Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Listsint> courses = new List<Listsint>
        {
            new Listsint { Id = 1, Names = "1 курс" },
            new Listsint { Id = 2, Names = "2 курс" },
            new Listsint { Id = 3, Names = "3 курс" },
            new Listsint { Id = 4, Names = "4 курс" },
            new Listsint { Id = 5, Names = "5 курс" },
            new Listsint { Id = 11, Names = "1 курс магистратуры" },
            new Listsint { Id = 12, Names = "2 курс магистратуры" }
        };

        IEnumerable<Listsint> groups = new List<Listsint>
        {
            new Listsint { Id = 1, Names = "1 группа" },
            new Listsint { Id = 2, Names = "2 группа" },
            new Listsint { Id = 3, Names = "3 группа" },
            new Listsint { Id = 4, Names = "4 группа" },
            new Listsint { Id = 5, Names = "5 группа" },
            new Listsint { Id = 6, Names = "6 группа" },
            new Listsint { Id = 61, Names = "61 группа" },
            new Listsint { Id = 62, Names = "62 группа" },
            new Listsint { Id = 71, Names = "71 группа" },
            new Listsint { Id = 9, Names = "9 группа" },
            new Listsint { Id = 91, Names = "91 группа" },
            new Listsint { Id = 10, Names = "10 группа" }
        };

        public IQueryable<NewTimetable> NewTimetable()
        {
            IQueryable<NewTimetable> NewTimetables = from timetable in ttdb.Timetables
                                                     join groups in ttdb.Groups on timetable.Id_Group equals groups.Id_Group
                                                     join teachers in ttdb.Teachers on timetable.Id_Teacher equals teachers.Id_Teacher
                                                     join lessons in ttdb.Lessons on timetable.Id_Lesson equals lessons.Id_Lesson
                                                     join classrooms in ttdb.Classrooms on timetable.Id_Class equals classrooms.Id_Class
                                                     select new NewTimetable
                                                     {
                                                         Id_Date = timetable.Id_Date,
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
            IQueryable <NewTimetable> pairtime = null;
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

        [Authorize(Roles = "Admin")]
        public IActionResult CreateTimetable()
        {
            ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
            ViewBag.Times = new SelectList(times, "Id", "Names");
            ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
            ViewBag.Courses = new SelectList(courses, "Id", "Names");
            ViewBag.Groups = new SelectList(groups, "Id", "Names");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTimetable(NewTimetable newtimetables)
        {
            var exceptiontime = NewTimetable();
            exceptiontime = exceptiontime.Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptionteacher = NewTimetable();
            exceptionteacher = exceptionteacher.Where(p => p.FIOteacher == newtimetables.FIOteacher)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptionclass = NewTimetable();
            exceptionclass = exceptionclass.Where(p => p.NumberClass == newtimetables.NumberClass)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptioninteg = NewTimetable();
            exceptioninteg = exceptioninteg.Where(p => p.Integrity == "Целое")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);
            var exceptionnum = NewTimetable();
            exceptionnum = exceptionnum.Where(p => p.Integrity == "Числитель")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);
            var exceptiondec = NewTimetable();
            exceptiondec = exceptiondec.Where(p => p.Integrity == "Знаменатель")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);

            if (exceptiontime.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View();
            }
            else if (exceptionteacher.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данный преподаватель занят в это время. Попробуйте изменить запрос.";
                return View();
            }
            else if (exceptionclass.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данная аудитория уже занята кем-то. Попробуйте изменить запрос.";
                return View();
            }
            else if((newtimetables.Integrity == "Числитель" || newtimetables.Integrity == "Знаменатель") && exceptioninteg.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View();
            }
            else if (newtimetables.Integrity == "Целое" && (exceptionnum.Any() || exceptiondec.Any()))
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View();
            }
            var id_les = ttdb.Lessons.Where(p => p.Name == newtimetables.NameLesson);
            var id_cl = ttdb.Classrooms.Where(p => p.NumberClass == newtimetables.NumberClass);
            var id_teac = ttdb.Teachers.Where(p => p.FIO == newtimetables.FIOteacher);
            var id_gr = ttdb.Groups.Where(p => p.NumberGroup == newtimetables.NumberGroup)
                    .Where(p => p.Course == newtimetables.Course);
            if (!id_gr.Any() || !id_teac.Any() || !id_cl.Any() || !id_les.Any())
            {
                if (!id_gr.Any())
                {
                    ViewBag.Exception = "Похоже выбранной группы не существует. Попробуйте изменить запрос.";
                }
                else if (!id_teac.Any())
                {
                    ViewBag.Exception = "Похоже выбранного преподавателя не существует. Попробуйте изменить запрос.";
                }
                else if (!id_cl.Any())
                {
                    ViewBag.Exception = "Похоже выбранной аудитории не существует. Попробуйте изменить запрос.";
                }
                else if (!id_les.Any())
                {
                    ViewBag.Exception = "Похоже выбранного предмета не существует. Попробуйте изменить запрос.";
                }
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                return View();
            }
            int id_group = id_gr.FirstOrDefault(p => p.NumberGroup == newtimetables.NumberGroup).Id_Group;
            int id_lesson = ttdb.Lessons.FirstOrDefault(p => p.Name == newtimetables.NameLesson).Id_Lesson;
            int id_class = ttdb.Classrooms.FirstOrDefault(p => p.NumberClass == newtimetables.NumberClass).Id_Class;
            int id_teacher = ttdb.Teachers.FirstOrDefault(p => p.FIO == newtimetables.FIOteacher).Id_Teacher;
            TimetableU timetable = new TimetableU
            {
                Id_Date = newtimetables.Id_Date,
                Week = newtimetables.Week,
                Time = newtimetables.Time,
                Integrity = newtimetables.Integrity,
                Id_Lesson = id_lesson,
                Id_Class = id_class,
                Id_Teacher = id_teacher,
                Id_Group = id_group
            };

            ttdb.Timetables.Add(timetable);
            await ttdb.SaveChangesAsync();
            return RedirectToAction("СhangeTimetable");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailsTimetable(int? id)
        {
            if (id != null)
            {
                IQueryable<NewTimetable> newtimetables = NewTimetable();
                var newtimetable = await newtimetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (newtimetable != null)
                    return View(newtimetable);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTimetable(int? id)
        {
            if (id != null)
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                IQueryable<NewTimetable> newtimetables = NewTimetable();
                var newtimetable = await newtimetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (newtimetable != null)
                    return View(newtimetable);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTimetable(NewTimetable newtimetables)
        {
            var exceptiontime = NewTimetable();
            exceptiontime = exceptiontime.Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptionteacher = NewTimetable();
            exceptionteacher = exceptionteacher.Where(p => p.FIOteacher == newtimetables.FIOteacher)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptionclass = NewTimetable();
            exceptionclass = exceptionclass.Where(p => p.NumberClass == newtimetables.NumberClass)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time)
                .Where(p => p.Integrity == newtimetables.Integrity);
            var exceptioninteg = NewTimetable();
            exceptioninteg = exceptioninteg.Where(p => p.Integrity == "Целое")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);
            var exceptionnum = NewTimetable();
            exceptionnum = exceptionnum.Where(p => p.Integrity == "Числитель")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);
            var exceptiondec = NewTimetable();
            exceptiondec = exceptiondec.Where(p => p.Integrity == "Знаменатель")
                .Where(p => p.NumberGroup == newtimetables.NumberGroup)
                .Where(p => p.Course == newtimetables.Course)
                .Where(p => p.Week == newtimetables.Week)
                .Where(p => p.Time == newtimetables.Time);

            if (exceptiontime.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View("СhangeTimetable");
            }
            else if (exceptionteacher.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данный преподаватель занят в это время. Попробуйте изменить запрос.";
                return View("СhangeTimetable");
            }
            else if (exceptionclass.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данная аудитория уже занята кем-то. Попробуйте изменить запрос.";
                return View("СhangeTimetable");
            }
            else if ((newtimetables.Integrity == "Числитель" || newtimetables.Integrity == "Знаменатель") && exceptioninteg.Any())
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View("СhangeTimetable");
            }
            else if (newtimetables.Integrity == "Целое" && (exceptionnum.Any() || exceptiondec.Any()))
            {
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                ViewBag.Exception = "Похоже данное время уже занято кем-то. Попробуйте изменить запрос.";
                return View("СhangeTimetable");
            }
            var id_les = ttdb.Lessons.Where(p => p.Name == newtimetables.NameLesson);
            var id_cl = ttdb.Classrooms.Where(p => p.NumberClass == newtimetables.NumberClass);
            var id_teac = ttdb.Teachers.Where(p => p.FIO == newtimetables.FIOteacher);
            var id_gr = ttdb.Groups.Where(p => p.NumberGroup == newtimetables.NumberGroup)
                    .Where(p => p.Course == newtimetables.Course);
            if (!id_gr.Any() || !id_teac.Any() || !id_cl.Any() || !id_les.Any())
            {
                if (!id_gr.Any())
                {
                    ViewBag.Exception = "Похоже выбранной группы не существует. Попробуйте изменить запрос.";
                }
                else if (!id_teac.Any())
                {
                    ViewBag.Exception = "Похоже выбранного преподавателя не существует. Попробуйте изменить запрос.";
                }
                else if (!id_cl.Any())
                {
                    ViewBag.Exception = "Похоже выбранной аудитории не существует. Попробуйте изменить запрос.";
                }
                else if (!id_les.Any())
                {
                    ViewBag.Exception = "Похоже выбранного предмета не существует. Попробуйте изменить запрос.";
                }
                ViewBag.Weeks = new SelectList(weeks, "Id", "Names");
                ViewBag.Times = new SelectList(times, "Id", "Names");
                ViewBag.Integrits = new SelectList(integrits, "Id", "Names");
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                return View("СhangeTimetable");
            }
            int id_group = id_gr.FirstOrDefault(p => p.NumberGroup == newtimetables.NumberGroup).Id_Group;
            int id_lesson = ttdb.Lessons.FirstOrDefault(p => p.Name == newtimetables.NameLesson).Id_Lesson;
            int id_class = ttdb.Classrooms.FirstOrDefault(p => p.NumberClass == newtimetables.NumberClass).Id_Class;
            int id_teacher = ttdb.Teachers.FirstOrDefault(p => p.FIO == newtimetables.FIOteacher).Id_Teacher;

            TimetableU timetable = new TimetableU
            {
                Id_Date = newtimetables.Id_Date,
                Week = newtimetables.Week,
                Time = newtimetables.Time,
                Integrity = newtimetables.Integrity,
                Id_Lesson = id_lesson,
                Id_Class = id_class,
                Id_Teacher = id_teacher,
                Id_Group = id_group
            };
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
                IQueryable<NewTimetable> newtimetables = NewTimetable();
                var newtimetable = await newtimetables.FirstOrDefaultAsync(p => p.Id_Date == id);
                if (newtimetable != null)
                    return View(newtimetable);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTimetable(int? id)
        {
            if (id != null)
            {
                TimetableU timetable = await ttdb.Timetables.FirstOrDefaultAsync(p => p.Id_Date == id);
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
