namespace Libro.Domain.Dtos
{
    public class CheckoutBookDto
    {
        public string ISBN { get; set; }
        public int PatronID { get; set; }
        public int LibrarianId { get; set; }
    }
}
