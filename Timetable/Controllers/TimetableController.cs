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
    }
}
