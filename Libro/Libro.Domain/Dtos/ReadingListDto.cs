namespace Libro.Domain.Dtos
{
    public class ReadingListDto
    {
        public int? ReadingListId { get; set; }
        public string Name { get; set; }
        public string? PatronId { get; set; }
    }
}
