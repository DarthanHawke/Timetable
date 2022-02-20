using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class GroupContext : DbContext
    {
        public DbSet<GroupU> Groups { get; set; }
        public GroupContext(DbContextOptions<GroupContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
