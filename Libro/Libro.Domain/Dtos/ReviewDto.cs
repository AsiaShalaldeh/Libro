namespace Libro.Domain.Dtos
{
    public class ReviewDto
    {
        public int PatronId { get; set; }
        public string BookId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
