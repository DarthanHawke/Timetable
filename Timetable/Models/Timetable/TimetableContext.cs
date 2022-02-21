using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class TimetableContext : DbContext
    {
        public DbSet<TimetableU> Timetables { get; set; }
        public DbSet<GroupU> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<LessonU> Lessons { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public TimetableContext(DbContextOptions<TimetableContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
