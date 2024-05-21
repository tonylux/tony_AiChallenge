using Auctions.Helpers;
using Auctions.Models;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class BidValidatorTests
{
    [Test]
    public void IsValidBid_ShouldReturnFalse_WhenListingIsNull()
    {
        // Arrange
        BidViewModel bidViewModel = new BidViewModel { Price = 100 };
        Listing listing = null;

        // Act
        var result = BidValidator.IsValidBid(bidViewModel, listing);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void IsValidBid_ShouldReturnTrue_WhenBidPriceIsHigherThanListingPriceAndHighestBid()
    {
        // Arrange
        BidViewModel bidViewModel = new BidViewModel { Price = 300 };
        Listing listing = new Listing
        {
            Price = 200,
            Bids = new List<Bid> { new Bid { Price = 250 } }
        };

        // Act
        var result = BidValidator.IsValidBid(bidViewModel, listing);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsValidBid_ShouldReturnTrue_WhenBidPriceIsHigherThanListingPriceAndNoPreviousBids()
    {
        // Arrange
        BidViewModel bidViewModel = new BidViewModel { Price = 300 };
        Listing listing = new Listing
        {
            Price = 200,
            Bids = new List<Bid>()
        };

        // Act
        var result = BidValidator.IsValidBid(bidViewModel, listing);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsValidBid_ShouldReturnFalse_WhenBidPriceIsLowerThanListingPrice()
    {
        // Arrange
        BidViewModel bidViewModel = new BidViewModel { Price = 100 };
        Listing listing = new Listing
        {
            Price = 200,
            Bids = new List<Bid>()
        };

        // Act
        var result = BidValidator.IsValidBid(bidViewModel, listing);

        // Assert
        Assert.IsFalse(result);
    }
}

