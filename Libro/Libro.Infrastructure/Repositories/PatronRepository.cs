using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;

namespace Libro.Infrastructure.Repositories
{
    public class PatronRepository : IPatronRepository
    {
        //private readonly DbContext _dbContext;
        private readonly IList<Patron> _patrons;


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
    }
}
