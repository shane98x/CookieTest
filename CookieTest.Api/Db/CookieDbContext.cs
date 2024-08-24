using CookieTest.Api.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace CookieTest.Api.Db
{
    public class CookieDbContext : DbContext
    {
        public CookieDbContext(DbContextOptions<CookieDbContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }

        public void Seed()
        {
            if (!Users.Any())
            {
                Users.AddRange(
                    new User { Username = "user1", Password = "pw1" },
                    new User { Username = "user2", Password = "pw2" },
                    new User { Username = "user3", Password = "pw3" },
                    new User { Username = "user4", Password = "pw4" },
                    new User { Username = "user5", Password = "pw5" },
                    new User { Username = "user6", Password = "pw6" },
                    new User { Username = "user7", Password = "pw7" },
                    new User { Username = "user8", Password = "pw8" },
                    new User { Username = "user9", Password = "pw9" },
                    new User { Username = "user10", Password = "pw10" }
                );
                SaveChanges();
            }
        }
    }
}
