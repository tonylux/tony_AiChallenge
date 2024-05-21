using Auctions.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class AuctionHub : Hub
{
    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly IApplicationDbContext _context;
    private static Timer _timer;
    private static bool _timerStarted;
    public AuctionHub(IHubContext<AuctionHub> hubContext, IApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
        if (!_timerStarted)
        {
            _timer = new Timer(CheckLatestBid, null, 0, 5000);
            _timerStarted = true;
        }
    }

    public void CheckLatestBid(object state)
    {
        var latestBid = _context.Bids.OrderByDescending(b => b.DatePlaced).FirstOrDefault();
        if (latestBid != null)
        {
            _hubContext.Clients.All.SendCoreAsync("ReceiveBid", new object[] { latestBid.Price });
        }
    }

}