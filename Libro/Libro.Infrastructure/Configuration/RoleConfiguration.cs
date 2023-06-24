using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libro.Infrastructure.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
            new IdentityRole
            {
                Id = "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                Name = "Patron",
                NormalizedName = "PATRON"
            },
            new IdentityRole
            {
                Id = "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                Name = "Librarian",
                NormalizedName = "LIBRARIAN"
            },
            new IdentityRole
            {
                Id = "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            });
        }
    }
}
