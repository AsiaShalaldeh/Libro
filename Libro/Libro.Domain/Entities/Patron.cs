namespace Libro.Domain.Entities
{
    public class Patron
    {
        public int PatronId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } // should be unique
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<ReadingList> ReadingLists { get; set; }
    }
}
