using Auctions.Data.Services;
using Auctions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace Auctions.Helpers;

public class ListingHelper : IListingHelper
{

    private readonly IListingsService _listingsService;
    private readonly IImageService _imageService;

    public ListingHelper(IListingsService listingsService, IImageService imageService )
    {
        _listingsService = listingsService;
        _imageService = imageService;
    }
    public async Task<Listing> CreateListing(ListingVM listing, IdentityUser identityUser)
    {
        if (listing.Image != null)
        {
            var listObj = new Listing
            {
                User = identityUser,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                IdentityUserId = identityUser?.Id,
                ImagePath = await _imageService.SaveImage(listing.Image)
            };

            return listObj;
        }
        return new Listing();
    }

    public async Task<Listing> GetListingById(int? id)
    {
        if (id == null)
        {
            return null;
        }

        var listing = await _listingsService.GetById(id);

        return listing;
    }
    public  PaginatedList<T> CreatePaginatedList<T>(List<T> items, int? pageNumber, int pageSize)
    {
        return  PaginatedList<T>.Create(items, pageNumber ?? 1, pageSize);
    }

}
