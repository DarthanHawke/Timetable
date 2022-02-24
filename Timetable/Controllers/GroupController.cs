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
    public class GroupController : Controller
    {
        private GroupContext gdb;

        public GroupController(GroupContext context)
        {
            gdb = context;
        }

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

        public async Task<IActionResult> Group(GroupSortState sortOrder = GroupSortState.CourseAsc)
        {
            IQueryable<GroupU> group = gdb.Groups;

            ViewData["CourseSort"] = sortOrder == GroupSortState.CourseAsc ? GroupSortState.CourseDesc : GroupSortState.CourseAsc;
            ViewData["NumberSort"] = sortOrder == GroupSortState.NumberAsc ? GroupSortState.NumberDesc : GroupSortState.NumberAsc;
            ViewData["NumberGroupSort"] = sortOrder == GroupSortState.NumberGroupAsc ? GroupSortState.NumberGroupDesc : GroupSortState.NumberGroupAsc;

            group = sortOrder switch
            {
                GroupSortState.CourseDesc => group.OrderByDescending(s => s.Course),
                GroupSortState.NumberAsc => group.OrderBy(s => s.Number),
                GroupSortState.NumberDesc => group.OrderByDescending(s => s.Number),
                GroupSortState.NumberGroupAsc => group.OrderBy(s => s.NumberGroup),
                GroupSortState.NumberGroupDesc => group.OrderByDescending(s => s.NumberGroup),
                _ => group.OrderBy(s => s.Course),
            };
            return View(await group.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateGroup()
        {
            ViewBag.Courses = new SelectList(courses, "Id", "Names");
            ViewBag.Groups = new SelectList(groups, "Id", "Names");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGroup(GroupU group)
        {
            gdb.Groups.Add(group);
            await gdb.SaveChangesAsync();
            return RedirectToAction("Group");
        }

        public async Task<IActionResult> DetailsGroup(int? id)
        {
            if (id != null)
            {
                GroupU group = await gdb.Groups.FirstOrDefaultAsync(p => p.Id_Group == id);
                if (group != null)
                    return View(group);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGroup(int? id)
        {
            if (id != null)
            {
                ViewBag.Courses = new SelectList(courses, "Id", "Names");
                ViewBag.Groups = new SelectList(groups, "Id", "Names");
                GroupU group = await gdb.Groups.FirstOrDefaultAsync(p => p.Id_Group == id);
                if (group != null)
                    return View(group);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGroup(GroupU group)
        {
            gdb.Groups.Update(group);
            await gdb.SaveChangesAsync();
            return RedirectToAction("Group");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteGroup")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                GroupU group = await gdb.Groups.FirstOrDefaultAsync(p => p.Id_Group == id);
                if (group != null)
                    return View(group);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGroup(int? id)
        {
            if (id != null)
            {
                GroupU group = new GroupU { Id_Group = id.Value };
                gdb.Entry(group).State = EntityState.Deleted;
                await gdb.SaveChangesAsync();
                return RedirectToAction("Group");
            }
            return NotFound();
        }
    }
}
