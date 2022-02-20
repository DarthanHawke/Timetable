using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class LessonContext : DbContext
    {
        public DbSet<LessonU> Lessons { get; set; }
        public LessonContext(DbContextOptions<LessonContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
