using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Dtos
{
    public class BookTransactionDto
    {
        [Required(ErrorMessage = "ISBN is required")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Patron ID is required")]
        public int PatronID { get; set; }
        
            //if (patronId <= 0)
            //    throw new ArgumentException("Invalid patron ID.", nameof(patronId));
    }
}
