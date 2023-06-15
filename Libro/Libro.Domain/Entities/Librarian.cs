using System.Text.Json.Serialization;

namespace Libro.Domain.Entities
{
    public class Librarian
    {
        public int LibrarianId { get; set; }
        public string Name { get; set; }

        //public ICollection<Author> AddedAuthors { get; set; }
        //public ICollection<Book> AddedBooks { get; set; }

    }
}
