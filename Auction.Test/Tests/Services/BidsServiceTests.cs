using Auctions.Models;
using Auctions.Data.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class BidsServiceTests
{
    private IApplicationDbContext _context;
    private DbSet<Bid> _dbSet;
    private BidsService _service;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _service = new BidsService(_context);
    }

    [Test]
    public async Task Add_ShouldCallAddAsyncAndSaveChangesAsync_WhenBidIsNotNull()
    {
        // Arrange
        var bid = new Bid { Price = 100 };
        var bids = Substitute.For<DbSet<Bid>, IQueryable<Bid>>();
        _context.Bids.Returns(bids);

        // Act
        await _service.Add(bid);

        // Assert
        await bids.Received().AddAsync(bid);
        await _context.Received(1).SaveChangesAsync();
    }


    [Test]
    public void Add_ShouldThrowArgumentException_WhenBidAmountIsLessThanOrEqualToZero()
    {
        // Arrange
        var bid = new Bid { Price = 0 };

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.Add(bid));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("Price"));
        Assert.That(ex.Message, Is.EqualTo("Le montant du bid doit être supérieur à zéro (Parameter 'Price')"));
    }

    [Test]
    public async Task GetAll_ShouldReturnAllBids()
    {
        // Arrange
        var bids = new List<Bid>
        {
            new Bid { Id = 1, Price = 100 },
            new Bid { Id = 2, Price = 200 },
            new Bid { Id = 3, Price = 300 }
        }.AsQueryable();
        var mockBids = CreateDbSetSubstitute(bids);
        _context.Bids.Returns(mockBids);
        // Act
        var result = _service.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo(bids));
    }

    private static DbSet<Bid> CreateDbSetSubstitute(IQueryable<Bid> queryableBids)
    {
        var dbSet = Substitute.For<DbSet<Bid>, IQueryable<Bid>>();
        ((IQueryable<Bid>)dbSet).Provider.Returns(queryableBids.Provider);
        ((IQueryable<Bid>)dbSet).Expression.Returns(queryableBids.Expression);
        ((IQueryable<Bid>)dbSet).ElementType.Returns(queryableBids.ElementType);
        ((IQueryable<Bid>)dbSet).GetEnumerator().Returns(queryableBids.GetEnumerator());

        return dbSet;
    }

    [Test]
    public void GetLatestBid_ReturnsLatestBid()
    {
        // Arrange
        var bids = new List<Bid>
        {
            new Bid { Price = 100, DatePlaced = DateTime.Now.AddDays(-1) },
            new Bid { Price = 200, DatePlaced = DateTime.Now }
        }.AsQueryable();

        var mockBids = CreateDbSetSubstitute(bids);
        _context.Bids.Returns(mockBids);

        // Act
        var latestBid = _service.GetLatestBid();

        // Assert
        Assert.That(latestBid, Is.EqualTo(bids.Last()));
    }
}