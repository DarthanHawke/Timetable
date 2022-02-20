using Microsoft.EntityFrameworkCore;


namespace Timetable.Models
{
    public class UserContext : DbContext
    {
        public DbSet<UserID> UserID { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
