using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patron>().HasData(
                new Patron { }
            );
        }
    }
}
