using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Timetable.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult AuthorizeIndex()
        {
            return RedirectToAction("Index");
        }

        public IActionResult index()
        {
            return View();
        }
        [Route("GetExceptionInfo")]
        [HttpGet]
        public IEnumerable<string> GetExceptionInfo()
        {
            string[] arrRetValues = null;
            if (arrRetValues.Length > 0)
            { }
            return arrRetValues;
        }
    }
}
