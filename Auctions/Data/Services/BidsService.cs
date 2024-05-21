using Auctions.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Data.Services
{
    public class BidsService : IBidsService
    {
        private readonly IApplicationDbContext _context;
        public BidsService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Bid bid)
        {
            if (bid == null)
            {
                throw new ArgumentNullException(nameof(bid), "Le bid ne peut pas être nul");
            }

            if (bid.Price <= 0)
            {
                throw new ArgumentException("Le montant du bid doit être supérieur à zéro", nameof(bid.Price));
            }
            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Bid> GetAll()
        {
            return _context.Bids
                           .Include(b => b.Listing)
                           .ThenInclude(l => l.User);
        }

    }
}
