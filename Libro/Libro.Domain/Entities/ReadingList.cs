namespace Libro.Domain.Entities
{
    public class ReadingList
    {
        public int ReadingListId { get; set; }
        public string Name { get; set; }
        public int PatronId { get; set; }
        public Patron Patron { get; set; }
        public ICollection<BookList> BookLists { get; set; }
    }
}
