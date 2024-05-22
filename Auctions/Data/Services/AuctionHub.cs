using Auctions.Data;
using Auctions.Data.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class AuctionHub : Hub
{
    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static Timer _timer;
    private static bool _timerStarted;
    public AuctionHub(IHubContext<AuctionHub> hubContext, IServiceScopeFactory serviceScopeFactory)
    {
        _hubContext = hubContext;
        _serviceScopeFactory = serviceScopeFactory;
        if (!_timerStarted)
        {
            _timer = new Timer(CheckLatestBid, null, 0, 5000);
            _timerStarted = true;
        }
    }

    public void CheckLatestBid(object state)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var bidService = scope.ServiceProvider.GetRequiredService<IBidsService>();
        var latestBid = bidService.GetLatestBid();
        if (latestBid != null)
        {
            _hubContext.Clients.All.SendCoreAsync("ReceiveBid", new object[] { latestBid.Price });
        }
    }

}