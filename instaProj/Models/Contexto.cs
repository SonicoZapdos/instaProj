using Microsoft.EntityFrameworkCore;
using instaProj.Models;

namespace instaProj.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
            Users = Set<User>();
            Archives = Set<Archive>();
            Comments = Set<Comment>();
            Ratings = Set<Rating>();
            SubComments = Set<SubComment>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Archive> Archives { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<SubComment> SubComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Archive>().HasKey(a => a.Id);

            modelBuilder.Entity<Comment>().HasKey(c => c.Id);

            modelBuilder.Entity<Rating>().HasKey(r => r.Id);

            modelBuilder.Entity<Archive>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.Id);

            modelBuilder.Entity<Comment>().HasOne(e => e.Archive).WithMany().HasForeignKey(e => e.Id);

            modelBuilder.Entity<Comment>().HasOne(e => e.User).WithMany().HasForeignKey(e => e.Id);

            modelBuilder.Entity<SubComment>().HasOne(e => e.Comment).WithMany().HasForeignKey(e => e.Id);
        }

        public DbSet<instaProj.Models.Follow>? Follow { get; set; }
    }
}
