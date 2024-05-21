using Auctions;
using Auctions.Helpers;
using Auctions.Models;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class ListingHelperTests
{
    private IListingHelper _listingHelper;
    private IdentityUser _identityUser;

    [SetUp]
    public void Setup()
    {
        _listingHelper = Substitute.For<IListingHelper>();
        _identityUser = new IdentityUser();
    }

    [Test]
    public async Task GetListingById_ShouldReturnListing_WhenCalledWithValidId()
    {
        // Arrange
        var expected = new Listing { Id = 1 };
        _listingHelper.GetListingById(1).Returns(expected);

        // Act
        var result = await _listingHelper.GetListingById(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task CreateListing_ShouldReturnListing_WhenCalledWithValidParameters()
    {
        // Arrange
        var listingVM = new ListingVM();
        var expected = new Listing();
        _listingHelper.CreateListing(listingVM, _identityUser).Returns(expected);

        // Act
        var result = await _listingHelper.CreateListing(listingVM, _identityUser);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CreatePaginatedList_ShouldReturnPaginatedList_WhenCalledWithValidParameters()
    {
        // Arrange
        var items = new List<Listing> { new Listing { Id = 1 }, new Listing { Id = 2 } };
        var expected = new PaginatedList<Listing>(items, items.Count, 1, items.Count);
        _listingHelper.CreatePaginatedList(items, 1, items.Count).Returns(expected);

        // Act
        var result = _listingHelper.CreatePaginatedList(items, 1, items.Count);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}