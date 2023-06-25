using Libro.Domain.Enums;

namespace Libro.Domain.Dtos
{
    public class BookRequest
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
    }
}
