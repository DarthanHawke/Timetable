using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class ClassroomContext : DbContext
    {
        public DbSet<Classroom> Classrooms { get; set; }
        public ClassroomContext(DbContextOptions<ClassroomContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
