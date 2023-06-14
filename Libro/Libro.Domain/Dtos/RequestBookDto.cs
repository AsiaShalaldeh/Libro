using Libro.Domain.Enums;

namespace Libro.Domain.Dtos
{
    public class RequestBookDto
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public Genre Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
    }
}
