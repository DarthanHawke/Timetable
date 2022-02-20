using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class TimetableContext : DbContext
    {
        public DbSet<TimetableU> Timetables { get; set; }
        public DbSet<CourseU> Courses { get; set; }
        public TimetableContext(DbContextOptions<TimetableContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimetableU>().HasNoKey();
        }
    }
}
