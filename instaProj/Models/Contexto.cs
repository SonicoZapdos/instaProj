using Microsoft.EntityFrameworkCore;
using instaProj.Models;

namespace instaProj.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
            Users = Set<User>();
            Follows = Set<Follow>();
            Posts = Set<Post>();
            Archives = Set<Archive>();
            Comments = Set<Comment>();
            SubComments = Set<SubComment>();
            Ratings = Set<Rating>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Archive> Archives { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            modelBuilder.Entity<Follow>().HasKey(f => f.Id);

            modelBuilder.Entity<Post>().HasKey(p => p.Id);

            modelBuilder.Entity<Archive>().HasKey(a => a.Id);

            modelBuilder.Entity<Comment>().HasKey(c => c.Id);

            modelBuilder.Entity<SubComment>().HasKey(s => s.Id);

            modelBuilder.Entity<Rating>().HasKey(r => r.Id);

            /* Conecxões */

            modelBuilder.Entity<Follow>().HasOne(f => f.User_Followed).WithMany().HasForeignKey(f => f.User_Id_Followed);

            modelBuilder.Entity<Follow>().HasOne(f => f.User_Following).WithMany().HasForeignKey(f => f.User_Id_Following);

            modelBuilder.Entity<Post>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.User_Id);

            modelBuilder.Entity<Archive>().HasOne(a => a.Post).WithMany().HasForeignKey(a => a.Post_Id);

            modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany().HasForeignKey(c => c.User_Id);

            modelBuilder.Entity<SubComment>().HasOne(s => s.Comment).WithMany().HasForeignKey(s => s.Comment_Id);

            modelBuilder.Entity<SubComment>().HasOne(s => s.User).WithMany().HasForeignKey(s => s.User_Id);

            modelBuilder.Entity<Rating>().HasOne(s => s.User).WithMany().HasForeignKey(s => s.User_Id);
        }
    }
}
