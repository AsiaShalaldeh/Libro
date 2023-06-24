using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Libro.Domain.Entities
{
    public class Librarian
    {
        [Key]
        public int LibrarianId { get; set; }
        public string Name { get; set; }
    }
}
