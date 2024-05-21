using Auctions.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class AuctionHubTests
{
    private AuctionHub? _auctionHub;
    private IHubContext<AuctionHub>? _hubContext;
    private IApplicationDbContext? _context;

    [SetUp]
    public void SetUp()
    {
        _hubContext = Substitute.For<IHubContext<AuctionHub>>();
        _context = Substitute.For<IApplicationDbContext>();
        var bids = new List<Bid>
        {
            new Bid { Price = 100, DatePlaced = DateTime.Now },
            new Bid { Price = 200, DatePlaced = DateTime.Now.AddDays(-1) }
        };
        var dbSet = CreateDbSetSubstitute(bids);
        _context?.Bids.Returns(dbSet);
        _auctionHub = new AuctionHub(_hubContext, _context);
    }

    [TearDown]
    public void TearDown()
    {
        _auctionHub?.Dispose();
    }
    private static DbSet<Bid> CreateDbSetSubstitute(List<Bid> bids)
    {
        var queryable = bids.AsQueryable();
        var dbSet = Substitute.For<DbSet<Bid>, IQueryable<Bid>>();
        ((IQueryable<Bid>)dbSet).Provider.Returns(queryable.Provider);
        ((IQueryable<Bid>)dbSet).Expression.Returns(queryable.Expression);
        ((IQueryable<Bid>)dbSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<Bid>)dbSet).GetEnumerator().Returns(queryable.GetEnumerator());

        return dbSet;
    }

    [Test]
    public void CheckLatestBid_WhenCalled_ShouldSendLatestBidPrice()
    {
        // Arrange
        var latestBid = _context?.Bids.OrderByDescending(b => b.DatePlaced).FirstOrDefault();
        // Act
        _auctionHub?.CheckLatestBid(new object());

        // Assert
        _hubContext?.Clients.All.Received().SendCoreAsync("ReceiveBid", Arg.Is<object[]>(o => (double)o[0] == latestBid.Price));
    }

    [Test]
    public void CheckLatestBid_NoBids_ShouldNotSendAnyMessage()
    {
        // Arrange
        var bids = new List<Bid>();
        var dbSet = CreateDbSetSubstitute(bids);
        _context?.Bids.Returns(dbSet);
        var latestBid = _context?.Bids.OrderByDescending(b => b.DatePlaced).FirstOrDefault();
        // Act
        _auctionHub?.CheckLatestBid(new object());

        // Assert
        _hubContext?.Clients.All.DidNotReceive().SendCoreAsync(Arg.Any<string>(), Arg.Any<object[]>());
    }
}
