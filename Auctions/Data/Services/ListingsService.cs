using Auctions.Models;
using Microsoft.EntityFrameworkCore;


namespace Auctions.Data.Services
{
    public class ListingsService : IListingsService
    {
        private readonly IApplicationDbContext _context;

        public ListingsService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Listing listing)
        {
            if (listing == null)
            {
                throw new ArgumentNullException(nameof(listing), "Le listing ne peut pas être nul");
            }
            await _context.Listings.AddAsync(listing);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Listing> GetAll()
        {
            var applicationDbContext = _context.Listings.Include(l => l.User);
            return applicationDbContext;
        }
       
        public async Task<Listing> GetById(int? id)
        {
            var listing = await _context.Listings
                .Include(l => l.User)
                .Include(l => l.Comments)
                .Include(l => l.Bids)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
           
            return listing;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
