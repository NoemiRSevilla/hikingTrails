using Microsoft.EntityFrameworkCore;

namespace hiking.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter alongcopy
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<User> trails {get;set;}
        public DbSet<Favorite> favorites {get;set;}
    }
}