﻿using Microsoft.AspNetCore.Mvc;
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
    public class LessonController : Controller
    {
        private LessonContext ldb;

        public LessonController(LessonContext context)
        {
            ldb = context;
        }

        public class Lists
        {
            public string Id { get; set; }
            public string Names { get; set; }
        }

        IEnumerable<Lists> typeLessons = new List<Lists>
        {
            new Lists { Id = "Лекция", Names = "Лекция" },
            new Lists { Id = "Практика", Names = "Практика" },
            new Lists { Id = "Лабораторная", Names = "Лабораторная" }
        };

        public async Task<IActionResult> Lesson(LessonSortState sortOrder = LessonSortState.NameAsc)
        {
            IQueryable<LessonU> lesson = ldb.Lessons;

            ViewData["TypeLessonSort"] = sortOrder == LessonSortState.TypeLessonAsc ? LessonSortState.TypeLessonDesc : LessonSortState.TypeLessonAsc;
            ViewData["NameSort"] = sortOrder == LessonSortState.NameAsc ? LessonSortState.NameDesc : LessonSortState.NameAsc;

            lesson = sortOrder switch
            {
                LessonSortState.NameDesc => lesson.OrderByDescending(s => s.Name),
                LessonSortState.TypeLessonAsc => lesson.OrderBy(s => s.TypeLesson),
                LessonSortState.TypeLessonDesc => lesson.OrderByDescending(s => s.TypeLesson),
                _ => lesson.OrderBy(s => s.Name),
            };
            return View(await lesson.AsNoTracking().ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateLesson()
        {
            ViewBag.TypeLessons = new SelectList(typeLessons, "Id", "Names");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLesson(LessonU lesson)
        {
            ldb.Lessons.Add(lesson);
            await ldb.SaveChangesAsync();
            return RedirectToAction("Lesson");
        }

        public async Task<IActionResult> DetailsLesson(int? id)
        {
            if (id != null)
            {
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditLesson(int? id)
        {
            if (id != null)
            {
                ViewBag.TypeLessons = new SelectList(typeLessons, "Id", "Names");
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditLesson(LessonU lesson)
        {
            ldb.Lessons.Update(lesson);
            await ldb.SaveChangesAsync();
            return RedirectToAction("Lesson");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ActionName("DeleteLesson")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                LessonU lesson = await ldb.Lessons.FirstOrDefaultAsync(p => p.Id_Lesson == id);
                if (lesson != null)
                    return View(lesson);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLesson(int? id)
        {
            if (id != null)
            {
                LessonU lesson = new LessonU { Id_Lesson = id.Value };
                ldb.Entry(lesson).State = EntityState.Deleted;
                await ldb.SaveChangesAsync();
                return RedirectToAction("Lesson");
            }
            return NotFound();
        }
    }
}
