using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libro.Infrastructure.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public string AdminRoleId { get; private set; }
        public string LibrarianRoleId { get; private set; }
        public string PatronRoleId { get; private set; }

        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            var adminRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            };

            var librarianRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Librarian",
                NormalizedName = "LIBRARIAN"
            };

            var patronRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Patron",
                NormalizedName = "PATRON"
            };

            AdminRoleId = adminRole.Id;
            LibrarianRoleId = librarianRole.Id;
            PatronRoleId = patronRole.Id;

            builder.HasData(adminRole, librarianRole, patronRole);
        }
    }
}
