namespace Libro.Domain.Entities
{
    public class Patron
    {
        public int PatronId { get; set; }
        public string Name { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<ReadingList> ReadingLists { get; set; }
    }
}
