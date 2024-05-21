using Auctions.Data;
using Auctions.Data.Services;
using Auctions.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class ListingsServiceTests
{
    private IApplicationDbContext _context;
    private DbSet<Listing> _dbSet;
    private ListingsService _service;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IApplicationDbContext>();
        _dbSet = Substitute.For<DbSet<Listing>, IQueryable<Listing>>();
        _context.Listings.Returns(_dbSet);
        _service = new ListingsService(_context);
    }

    [Test]
    public async Task Add_ShouldCallAddAsyncAndSaveChangesAsync_WhenListingIsNotNull()
    {
        // Arrange
        var listing = new Listing();

        // Act
        await _service.Add(listing);

        // Assert
        await _dbSet.Received(1).AddAsync(listing);
        await _context.Received(1).SaveChangesAsync();
    }

    [Test]
    public void Add_ShouldThrowArgumentNullException_WhenListingIsNull()
    {
        // Arrange
        Listing listing = null;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _service.Add(listing));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("listing"));
        Assert.That(ex.Message, Is.EqualTo("Le listing ne peut pas être nul (Parameter 'listing')"));
    }

    [Test]
    public async Task GetAll_ShouldReturnAllListings()
    {
        // Arrange
        var listings = new List<Listing>
    {
        new Listing { Id = 1, Title = "Listing 1" },
        new Listing { Id = 2, Title = "Listing 2" },
        new Listing { Id = 3, Title = "Listing 3" }
    }.AsQueryable();

        var mockSet = Substitute.For<DbSet<Listing>, IQueryable<Listing>>();
        ((IQueryable<Listing>)mockSet).Provider.Returns(listings.Provider);
        ((IQueryable<Listing>)mockSet).Expression.Returns(listings.Expression);
        ((IQueryable<Listing>)mockSet).ElementType.Returns(listings.ElementType);
        ((IQueryable<Listing>)mockSet).GetEnumerator().Returns(listings.GetEnumerator());

        _context.Listings.Returns(mockSet);

        // Act
        var result = _service.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo(listings));
    }
}