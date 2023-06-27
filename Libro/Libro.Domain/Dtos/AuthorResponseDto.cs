namespace Libro.Domain.Dtos
{
    public class AuthorResponseDto
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public ICollection<BookDto> Books { get; set; }

    }
}
