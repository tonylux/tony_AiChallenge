using Auctions.Models;

namespace Auctions.Helpers;

public class BidValidator
{
    public static bool IsValidBid(BidViewModel bidViewModel, Listing listing)
    {
        if (listing == null)
        {
            return false;
        }
        if (listing.Bids.Count != 0)
        {
            // Si la liste de mises n'est pas vide, vérifie si la nouvelle mise est supérieure au prix annoncé et à la mise la plus récente
            if (bidViewModel.Price > listing.Price && bidViewModel.Price > listing.Bids.Max(b => b.Price))
            {
                return true;
            }
        }
        else
        {
            if (bidViewModel.Price > listing.Price)
            {
                return true;
            }
        }
        return false;
    }
}
