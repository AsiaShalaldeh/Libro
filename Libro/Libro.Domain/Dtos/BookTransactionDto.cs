using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Dtos
{
    public class BookTransactionDto
    {
        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Patron ID is required")]
        public string PatronID { get; set; }
    }
}
