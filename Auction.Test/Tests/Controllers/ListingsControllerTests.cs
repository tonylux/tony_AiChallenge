
using Auctions;
using Auctions.Controllers;
using Auctions.Data.Services;
using Auctions.Helpers;
using Auctions.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Auction.Test.Tests.Controllers;

[TestFixture]
public class ListingsControllerTests
{
    private IListingsService _mockListingsService;
    private IWebHostEnvironment _mockWebHostEnvironment;
    private IBidsService _mockBidsService;
    private ICommentsService _mockCommentsService;
    private IUserManager _mockUserManager;
    private IListingHelper _mockListingHelper;
    private ICacheService _mockCacheService;
    private ListingsController _controller;



    [TearDown]
    public void TearDown()
    {
        _controller.Dispose();
    }

    [SetUp]
    public void SetUp()
    {
        _mockListingsService = Substitute.For<IListingsService>();
        _mockWebHostEnvironment = Substitute.For<IWebHostEnvironment>();
        _mockBidsService = Substitute.For<IBidsService>();
        _mockCommentsService = Substitute.For<ICommentsService>();
        _mockUserManager = Substitute.For<IUserManager>();
        _mockListingHelper = Substitute.For<IListingHelper>();
        _mockCacheService = Substitute.For<ICacheService>();
        _controller = new ListingsController(_mockListingsService, _mockWebHostEnvironment, _mockBidsService, _mockCommentsService, _mockUserManager, _mockListingHelper, _mockCacheService);
    }

    [Test]
    public async Task Index_ReturnsViewResult_WithAListOfListings()
    {
        // Arrange
        var listings = new List<Listing> { new Listing(), new Listing() };
        _mockCacheService.Retrieve<List<Listing>>("AllListings").Returns(listings);
        var paginatedListings = new PaginatedList<Listing>(listings, listings.Count, 1, listings.Count);
        _mockListingHelper.CreatePaginatedList(Arg.Any<List<Listing>>(), Arg.Any<int>(), Arg.Any<int>()).Returns(paginatedListings);

        // Act
        var result = await _controller.Index(null, null);

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.ViewData.Model, Is.InstanceOf<PaginatedList<Listing>>());
        var model = viewResult.ViewData.Model as PaginatedList<Listing>;
        Assert.That(model.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task Details_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Details(null);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Details_ReturnsNotFound_WhenListingDoesNotExist()
    {
        // Arrange
        var id = 1;
        _mockListingsService.GetById(id).Returns((Listing)null);
        _mockListingHelper.GetListingById(id).Returns(Task.FromResult<Listing>(null));

        // Act
        var result = await _controller.Details(id);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenModelStateIsValidAndImageIsNull()
    {
        // Arrange
        var listingVM = new ListingVM { Title = "Test" };
        var listing = new Listing { Title = listingVM.Title };
        _mockListingHelper.CreateListing(listingVM, Arg.Any<IdentityUser>()).Returns(listing);

        // Act
        var result = await _controller.Create(listingVM);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task AddBid_ReturnsBadRequest_WhenModelStateIsValidAndBidIsInvalid()
    {
        // Arrange
        var bidViewModel = new BidViewModel
        {
            Price = 0,
            ListingId = 1,
            Id = 0,

        };

        var listing = new Listing
        {
            Price = 100,
            Title = "Test Listing",
            Description = "This is a test listing",
            Bids = new List<Bid>
            {
                new Bid()
            },

        };

        _mockListingHelper.GetListingById(bidViewModel.ListingId).Returns(Task.FromResult(listing));

        // Act
        var result = await _controller.AddBid(bidViewModel);

        // Assert
        // Assert
        if (!BidValidator.IsValidBid(bidViewModel, listing))
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewName, Is.EqualTo("Details"));
            Assert.That(viewResult.Model, Is.EqualTo(listing));
            Assert.That(_controller.ModelState["Price"].Errors, Is.Not.Empty);
        }
        else
        {
            Assert.Fail("BidValidator.IsValidBid a renvoyé true alors qu'il devrait renvoyer false");
        }
    }

    [Test]
    public async Task CloseBidding_ReturnsBadRequest_WhenListingDoesNotExist()
    {
        // Arrange
        var id = 1;
        _mockListingsService.GetById(id).Returns((Listing)null);

        // Act
        var result = await _controller.CloseBidding(id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task CloseBidding_ReturnsOk_WhenListingExistsAndClosingIsSuccessful()
    {
        // Arrange
        var id = 1;
        var listing = new Listing { Id = id, IsSold = true };
        _mockListingsService.GetById(id).Returns(listing);
        _mockListingHelper.GetListingById(id).Returns(Task.FromResult(listing));
        // Act
        var result = await _controller.CloseBidding(id);
        var viewResult = result as ViewResult;
        // Assert
        Assert.That(viewResult.ViewName, Is.EqualTo("Details"));
        Assert.That(_controller.ModelState["Price"]?.Errors, Is.Not.Empty);

    }


    [Test]
    public async Task AddComment_ReturnsViewResult_WhenModelIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Text", "Erreur de test");

        // Act
        var result = await _controller.AddComment(new Comment());

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [Test]
    public async Task CloseBidding_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var id = 1;
        _mockListingHelper.GetListingById(id).Returns<Task<Listing>>(x => { throw new Exception(); });

        // Act
        var result = await _controller.CloseBidding(id);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    [Test]
    public async Task AddBid_ReturnsViewResult_WhenModelIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Price", "Erreur de test");

        // Act
        var result = await _controller.AddBid(new BidViewModel());

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

}
