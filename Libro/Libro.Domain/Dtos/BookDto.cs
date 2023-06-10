using Libro.Domain.Enums;

namespace Libro.Domain.Dtos
{
    public class BookDto
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicatinDate { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }
        public string AuthorName { get; set; } 
    }
}
