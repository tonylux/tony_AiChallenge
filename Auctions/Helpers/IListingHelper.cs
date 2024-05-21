using Auctions.Models;
using Microsoft.AspNetCore.Identity;

namespace Auctions.Helpers;

public interface IListingHelper
{
    Task<Listing> GetListingById(int? id);
    Task<Listing> CreateListing(ListingVM listing, IdentityUser identityUser);
    PaginatedList<T> CreatePaginatedList<T>(List<T> items, int? pageNumber, int pageSize);
}
