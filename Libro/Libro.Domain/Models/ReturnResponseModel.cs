﻿namespace Libro.Domain.Models
{
    public class ReturnResponseModel
    {
        public string BookId { get; set; }
        public string PatronId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalFee { get; set; }
    }
}
