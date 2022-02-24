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
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private UserContext tdb;

        public UserController(UserContext context)
        {
            tdb = context;
        }

        public class Lists
        {
            public string Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Lists> accesses = new List<Lists>
        {
            new Lists { Id = "Admin", Names = "Админ" },
            new Lists { Id = "User", Names = "Пользователь" }
        };


        public async Task<IActionResult> User(UserSortState sortOrder = UserSortState.Id_UserAsc)
        {
            IQueryable<UserID> user = tdb.UserID;

            ViewData["FIOSort"] = sortOrder == UserSortState.Id_UserAsc ? UserSortState.Id_UserDesc : UserSortState.Id_UserAsc;
            ViewData["SpecializationSort"] = sortOrder == UserSortState.AccessAsc ? UserSortState.AccessDesc : UserSortState.AccessAsc;

            user = sortOrder switch
            {
                UserSortState.Id_UserDesc => user.OrderByDescending(s => s.Id_User),
                UserSortState.AccessAsc => user.OrderBy(s => s.Access),
                UserSortState.AccessDesc => user.OrderByDescending(s => s.Access),
                _ => user.OrderBy(s => s.Id_User),
            };
            return View(await user.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            ViewBag.Accesses = new SelectList(accesses, "Id", "Names");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(UserID user)
        {
            tdb.UserID.Add(user);
            await tdb.SaveChangesAsync();
            return RedirectToAction("User");
        }

        public async Task<IActionResult> DetailsUser(int? id)
        {
            if (id != null)
            {
                UserID user = await tdb.UserID.FirstOrDefaultAsync(p => p.Id_User == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id != null)
            {
                ViewBag.Accesses = new SelectList(accesses, "Id", "Names");
                UserID user = await tdb.UserID.FirstOrDefaultAsync(p => p.Id_User == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(UserID user)
        {
            tdb.UserID.Update(user);
            await tdb.SaveChangesAsync();
            return RedirectToAction("User");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteUser")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                UserID user = await tdb.UserID.FirstOrDefaultAsync(p => p.Id_User == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id != null)
            {
                UserID user = new UserID { Id_User = id.Value };
                tdb.Entry(user).State = EntityState.Deleted;
                await tdb.SaveChangesAsync();
                return RedirectToAction("User");
            }
            return NotFound();
        }
    }
}
