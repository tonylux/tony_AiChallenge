using Auctions.Data.Services;
using Auctions.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Auction.Test.Tests.Services;

[TestFixture]
public class AuctionHubTests
{
    private AuctionHub? _auctionHub;
    private IHubContext<AuctionHub> _hubContext;
    private IBidsService _bidsService;
    private IClientProxy _clientProxy;
    private IServiceScopeFactory _serviceScopeFactory;
    private IServiceScope _serviceScope;
  

    [SetUp]
    public void SetUp()
    {
        _hubContext = Substitute.For<IHubContext<AuctionHub>>();
        _bidsService = Substitute.For<IBidsService>();
        _clientProxy = Substitute.For<IClientProxy>();
      
        _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        _serviceScope = Substitute.For<IServiceScope>();
        _serviceScope.ServiceProvider.GetService(typeof(IBidsService)).Returns(_bidsService);
        _serviceScopeFactory.CreateScope().Returns(_serviceScope);
        _hubContext.Clients.All.Returns(_clientProxy);
        _auctionHub = new AuctionHub(_hubContext, _serviceScopeFactory);
    }

    [TearDown]
    public void TearDown()
    {
        _auctionHub?.Dispose();
        _serviceScope.Dispose();
    }

    [Test]
    public void CheckLatestBid_WhenCalled_ShouldSendLatestBidPrice()
    {
        // Arrange
        var latestBid = new Bid { Price = 200, DatePlaced = DateTime.Now };
        _bidsService.GetLatestBid().Returns(latestBid);

        // Act
        _auctionHub.CheckLatestBid(null);

        // Assert
        _clientProxy.Received().SendCoreAsync("ReceiveBid", Arg.Is<object[]>(o => (double)o[0] == latestBid.Price), Arg.Any<CancellationToken>());
    }

    [Test]
    public void CheckLatestBid_NoBids_ShouldNotSendAnyMessage()
    {
        // Arrange
        _bidsService.GetLatestBid().Returns((Bid)null);
        // Act
        _auctionHub.CheckLatestBid(null);

        // Assert
        _clientProxy.DidNotReceive().SendCoreAsync(Arg.Any<string>(), Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
