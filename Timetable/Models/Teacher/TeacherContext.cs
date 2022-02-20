using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class TeacherContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public TeacherContext(DbContextOptions<TeacherContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
