using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Book
    {
        [Key]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicatinDate { get; set; }
        public Genre Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<BookList> BookLists { get; set; }

    }
}
