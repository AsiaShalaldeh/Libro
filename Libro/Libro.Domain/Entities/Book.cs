using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Entities
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public Genre Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Checkout> Checkouts { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<BookList> BookLists { get; set; }
    }
}
