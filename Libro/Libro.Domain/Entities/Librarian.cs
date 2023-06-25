using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Libro.Domain.Entities
{
    public class Librarian
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string LibrarianId { get; set; }
        public string Name { get; set; }
    }
}
