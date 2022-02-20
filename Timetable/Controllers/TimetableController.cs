using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
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
            ViewBag.courseset = model.PeriodCourses.ToString();
            ViewBag.groupeset = model.PeriodGroups.ToString();
            return View("СhoiceTimetable");
        }
    }
}
