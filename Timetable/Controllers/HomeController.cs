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

        public IActionResult index()
        {
            return View();
        }
    }
}
