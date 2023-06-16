﻿using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Book
    {
        [Key]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public Genre Genre { get; set; }
        public bool IsAvailable { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        //public ICollection<Review> Reviews { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<BookList> BookLists { get; set; }

    }
}
