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
        _dbSet = Substitute.For<DbSet<Bid>, IQueryable<Bid>>();
        _context.Bids.Returns(_dbSet);
        _service = new BidsService(_context);
    }

    [Test]
    public async Task Add_ShouldCallAddAsyncAndSaveChangesAsync_WhenBidIsNotNull()
    {
        // Arrange
        var bid = new Bid { Price = 100 };

        // Act
        await _service.Add(bid);

        // Assert
        await _dbSet.Received(1).AddAsync(bid);
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

        var mockSet = Substitute.For<DbSet<Bid>, IQueryable<Bid>>();
        ((IQueryable<Bid>)mockSet).Provider.Returns(bids.Provider);
        ((IQueryable<Bid>)mockSet).Expression.Returns(bids.Expression);
        ((IQueryable<Bid>)mockSet).ElementType.Returns(bids.ElementType);
        ((IQueryable<Bid>)mockSet).GetEnumerator().Returns(bids.GetEnumerator());

        _context.Bids.Returns(mockSet);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo(bids));
    }
}