using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class PatronMockData
    {
        public static IList<Patron> patrons = new List<Patron>();
        public static Patron GetPatron(string patronId)
        {
            return patrons.FirstOrDefault(p => p.PatronId.Equals(patronId));
        }

        public static Patron ReturnPatron()
        {
            return new Patron
            {
                PatronId = "2",
                Name = "John Doe",
                Email = "johndoe@gmail.com",
                Reviews = new List<Review>
                    {
                        new Review { ReviewId = 3, Rating = 4, Comment = "Great book!", BookId = "1" },
                        new Review { ReviewId = 4, Rating = 5, Comment = "Highly recommended!", BookId = "1" }
                    },
                ReservedBooks = new List<Reservation>
                    {
                        new Reservation
                        {
                            ReservationId = "1",
                            BookId = "1",
                            PatronId = "1",
                            ReservationDate = DateTime.Now
                        }
                    },
                CheckedoutBooks = new List<Checkout>
                    {
                        new Checkout
                        {
                            CheckoutId = "1",
                            BookId = "2",
                            PatronId = "1",
                            CheckoutDate = DateTime.Now,
                            DueDate = DateTime.Now.AddDays(7),
                            IsReturned = false,
                            ReturnDate = DateTime.MinValue,
                            TotalFee = 0.0m
                        }
                    },
                ReadingLists = new List<ReadingList>
                    {
                        new ReadingList { ReadingListId = 3, Name = "Reading List 1" },
                        new ReadingList { ReadingListId = 4, Name = "Reading List 2" }
                    }
            };
        }
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            if (!dbContext.Patrons.Any())
            {
                var patron1 = new Patron
                {
                    PatronId = "1",
                    Name = "John Doe",
                    Email = "doe@gmail.com",
                    Reviews = new List<Review>
                    {
                        new Review { ReviewId = 1, Rating = 4, Comment = "Great book!", BookId = "1" },
                        new Review { ReviewId = 2, Rating = 5, Comment = "Highly recommended!", BookId = "2" }
                    },
                    ReservedBooks = new List<Reservation>(),
                    CheckedoutBooks = new List<Checkout>(),
                    ReadingLists = new List<ReadingList>
                    {
                        new ReadingList { ReadingListId = 1, Name = "Reading List 1" },
                        new ReadingList { ReadingListId = 2, Name = "Reading List 2" }
                    }
                };

                var patron2 = new Patron
                {
                    PatronId = "2",
                    Name = "John Doe",
                    Email = "johndoe@gmail.com",
                    Reviews = new List<Review>
                    {
                        new Review { ReviewId = 3, Rating = 4, Comment = "Great book!", BookId = "1" },
                        new Review { ReviewId = 4, Rating = 5, Comment = "Highly recommended!", BookId = "1" }
                    },
                    ReservedBooks = new List<Reservation>
                    {
                        new Reservation
                        {
                            ReservationId = "1",
                            BookId = "1",
                            PatronId = "1",
                            ReservationDate = DateTime.Now
                        }
                    },
                    CheckedoutBooks = new List<Checkout>
                    {
                        new Checkout
                        {
                            CheckoutId = "1",
                            BookId = "2",
                            PatronId = "1",
                            CheckoutDate = DateTime.Now,
                            DueDate = DateTime.Now.AddDays(7),
                            IsReturned = false,
                            ReturnDate = DateTime.MinValue,
                            TotalFee = 0.0m
                        }
                    },
                    ReadingLists = new List<ReadingList>
                    {
                        new ReadingList { ReadingListId = 3, Name = "Reading List 1" },
                        new ReadingList { ReadingListId = 4, Name = "Reading List 2" }
                    }
                };

                dbContext.Patrons.Add(patron1);
                dbContext.Patrons.Add(patron2);
                patrons.Add(patron1);
                patrons.Add(patron2);
                dbContext.SaveChanges();
            }
        }
    }
}
