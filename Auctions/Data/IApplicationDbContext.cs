using Auctions.Models;
using Microsoft.EntityFrameworkCore;

public interface IApplicationDbContext
{
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Bid> Bids { get; set; }
    public DbSet<Comment> Comments { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
