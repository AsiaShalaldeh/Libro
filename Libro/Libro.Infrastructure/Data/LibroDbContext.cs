using Libro.Domain.Entities;
using Libro.Infrastructure.Configuration;
using Libro.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Libro.Infrastructure.Data
{
    public class LibroDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration _configuration;
        public LibroDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("Libro");
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookList>()
            .HasKey(bl => new { bl.ReadingListId, bl.BookId });

            modelBuilder.Entity<BookList>()
                .HasOne(bl => bl.ReadingList)
                .WithMany(rl => rl.BookLists)
                .HasForeignKey(bl => bl.ReadingListId);

            modelBuilder.Entity<BookList>()
                .HasOne(bl => bl.Book)
                .WithMany(b => b.BookLists)
                .HasForeignKey(bl => bl.BookId);


            modelBuilder.Entity<Checkout>()
            .Property(c => c.TotalFee)
            .HasColumnType("decimal(18, 2)");

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.Seed();
        }

        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReadingList> ReadingLists { get; set; }

    }
}
