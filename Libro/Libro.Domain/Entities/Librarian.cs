using System.Text.Json.Serialization;

namespace Libro.Domain.Entities
{
    public class Librarian
    {
        public int LibrarianId { get; set; }
        public string Name { get; set; }
        //public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
