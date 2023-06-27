using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Dtos
{
    public class ReviewDto
    {
        public int? reviewId { get; set; }
        public string? BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? PatronId { get; set; }
        //public string? PatronName { get; set; }

        [Range(1, 5, ErrorMessage = "The Rating must be between 1 and 5!")]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
