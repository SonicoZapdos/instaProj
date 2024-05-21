using Microsoft.EntityFrameworkCore;

namespace instaProj.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
            Users = Set<User>();
            Archives = Set<Archive>();
            Comments = Set<Comment>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Archive> Archives { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Archive>().HasKey(a => a.Id);

            modelBuilder.Entity<Archive>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.Id);

        }
    }
}
