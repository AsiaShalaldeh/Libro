using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        //private readonly DbContext _dbContext;
        private readonly IList<Patron> _patrons;
        private readonly IList<Checkout> _transactions;


        public PatronRepository()
        {
            _patrons = new List<Patron>
            {
                new Patron
                {
                    PatronId = 1,
                    Name = "John Doe"
                },
                new Patron
                {
                    PatronId = 2,
                    Name = "Jane Smith"
                },
            };
            _transactions = new List<Checkout>();
        }

        public Patron GetPatronByIdAsync(int patronId)
        {
            return _patrons
                //.Include(p => p.Transactions)
                .FirstOrDefault(p => p.PatronId == patronId);
        }

        public Patron UpdatePatronAsync(Patron patron)
        {
            //_patrons.Update(patron);
            //await _dbContext.SaveChangesAsync();
            return patron;
        }
        public IEnumerable<Checkout> GetBorrowingHistoryAsync(int patronId)
        {
            var patron = _patrons.Where(p => p.PatronId == patronId).FirstOrDefault();
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }

            var transactions = _transactions
                //.Include(t => t.Book)
                .Where(t => t.PatronId == patronId)
                .ToList();
            List<Checkout> checkouts = transactions
            .OfType<Checkout>()
            .Select(t => new Checkout
            {
                BookId = t.BookId,
                PatronId = t.PatronId,
                CheckoutDate = t.CheckoutDate,
                DueDate = t.DueDate,
                IsReturned = t.IsReturned,
                ReturnDate = t.ReturnDate
            })
            .ToList();

            return checkouts;
        }
    }
}
