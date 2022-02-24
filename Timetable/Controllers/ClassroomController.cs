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
    public class ClassroomController : Controller
    {
        private ClassroomContext cdb;

        public ClassroomController(ClassroomContext context)
        {
            cdb = context;
        }

        public class Lists
        {
            public string Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Lists> typeclasses = new List<Lists>
        {
            new Lists { Id = "Лекционная", Names = "Лекционная" },
            new Lists { Id = "Семинарная", Names = "Семинарная" },
            new Lists { Id = "Лабораторная", Names = "Лабораторная" }
        };

        public class Listsint
        {
            public int Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Listsint> equipments = new List<Listsint>
        {
            new Listsint { Id = 1, Names = "Да" },
            new Listsint { Id = 0, Names = "Нет" }
        };

        public async Task<IActionResult> Classroom(ClassroomSortState sortOrder = ClassroomSortState.NumberClassAsc)
        {
            IQueryable<Classroom> classroom = cdb.Classrooms;

            ViewData["NumberClassSort"] = sortOrder == ClassroomSortState.NumberClassAsc ? ClassroomSortState.NumberClassDesc : ClassroomSortState.NumberClassAsc;
            ViewData["TypeClassSort"] = sortOrder == ClassroomSortState.TypeClassAsc ? ClassroomSortState.TypeClassDesc : ClassroomSortState.TypeClassAsc;
            ViewData["CapacitySort"] = sortOrder == ClassroomSortState.CapacityAsc ? ClassroomSortState.CapacityDesc : ClassroomSortState.CapacityAsc;

            classroom = sortOrder switch
            {
                ClassroomSortState.NumberClassDesc => classroom.OrderByDescending(s => s.NumberClass),
                ClassroomSortState.TypeClassAsc => classroom.OrderBy(s => s.TypeClass),
                ClassroomSortState.TypeClassDesc => classroom.OrderByDescending(s => s.TypeClass),
                ClassroomSortState.CapacityAsc => classroom.OrderBy(s => s.Capacity),
                ClassroomSortState.CapacityDesc => classroom.OrderByDescending(s => s.Capacity),
                _ => classroom.OrderBy(s => s.NumberClass),
            };
            return View(await classroom.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateClassroom()
        {
            ViewBag.Typeclasses = new SelectList(typeclasses, "Id", "Names");
            ViewBag.Equipments = new SelectList(equipments, "Id", "Names");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClassroom(Classroom classroom)
        {
            cdb.Classrooms.Add(classroom);
            await cdb.SaveChangesAsync();
            return RedirectToAction("Classroom");
        }

        public async Task<IActionResult> DetailsClassroom(int? id)
        {
            if (id != null)
            {
                Classroom classroom = await cdb.Classrooms.FirstOrDefaultAsync(p => p.Id_Class == id);
                if (classroom != null)
                    return View(classroom);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditClassroom(int? id)
        {
            if (id != null)
            {
                ViewBag.Typeclasses = new SelectList(typeclasses, "Id", "Names");
                ViewBag.Equipments = new SelectList(equipments, "Id", "Names");
                Classroom classroom = await cdb.Classrooms.FirstOrDefaultAsync(p => p.Id_Class == id);
                if (classroom != null)
                    return View(classroom);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditClassroom(Classroom classroom)
        {
            cdb.Classrooms.Update(classroom);
            await cdb.SaveChangesAsync();
            return RedirectToAction("Classroom");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteClassroom")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Classroom classroom = await cdb.Classrooms.FirstOrDefaultAsync(p => p.Id_Class == id);
                if (classroom != null)
                    return View(classroom);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClassroom(int? id)
        {
            if (id != null)
            {
                Classroom classroom = new Classroom { Id_Class = id.Value };
                cdb.Entry(classroom).State = EntityState.Deleted;
                await cdb.SaveChangesAsync();
                return RedirectToAction("Classroom");
            }
            return NotFound();
        }
    }
}
