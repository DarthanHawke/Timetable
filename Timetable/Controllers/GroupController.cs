using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Timetable.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        public IActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
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

        public async Task<IActionResult> EditGroup(int? id)
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
        public async Task<IActionResult> EditGroup(GroupU group)
        {
            gdb.Groups.Update(group);
            await gdb.SaveChangesAsync();
            return RedirectToAction("Group");
        }

        [HttpGet]
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
