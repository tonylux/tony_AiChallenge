using Auctions.Data.Services;
using Auctions.Helpers;
using Auctions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace Auctions.Controllers
{
    [Authorize]
    public class ListingsController : Controller
    {
        private readonly IListingsService _listingsService;
        private readonly IBidsService _bidsService;
        private readonly ICommentsService _commentsService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserManager _userManager;
        private readonly IListingHelper _listingHelper;
        private readonly ICacheService _cacheService;

        private const int PageSize = 3;
        public ListingsController(IListingsService listingsService, IWebHostEnvironment webHostEnvironment, IBidsService bidsService, ICommentsService commentsService, IUserManager userManager, IListingHelper listingHelper, ICacheService cacheService)
        {
            _listingsService = listingsService;
            _webHostEnvironment = webHostEnvironment;
            _bidsService = bidsService;
            _commentsService = commentsService;
            _userManager = userManager;
            _listingHelper = listingHelper;
            _cacheService = cacheService;
        }

        [AllowAnonymous]
        // GET: Listings
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            var cacheKey = "AllListings";
            var listings = _cacheService.Retrieve<List<Listing>>(cacheKey);
            if (listings == null)
            {
                var listingsQuery = _listingsService.GetAll();
                if (listingsQuery == null)
                {
                    return NotFound();
                }

                listings = listingsQuery.ToList();
                _cacheService.Store(cacheKey, listings, TimeSpan.FromMinutes(120));
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                listings = listings.Where(a => a.Title.Contains(searchString) && !a.IsSold).ToList();

            }

            return View(_listingHelper.CreatePaginatedList(listings, pageNumber ?? 1, PageSize));
        }
        public async Task<IActionResult> MyListings(int? pageNumber)
        {
            var cacheKey = "AllListings";
            var listings = _cacheService.Retrieve<List<Listing>>(cacheKey);
            if (listings == null)
            {
                var listingsQuery = _listingsService.GetAll();
                if (listingsQuery == null)
                {
                    return NotFound();
                }

                listings = listingsQuery.ToList();
                _cacheService.Store(cacheKey, listings, TimeSpan.FromMinutes(120));
            }
            var userId = _userManager.GetUserIdAsync(User);
            return View("Index", _listingHelper.CreatePaginatedList(listings.Where(l => l.IdentityUserId == userId).ToList(), pageNumber ?? 1, PageSize));
        }
        public async Task<IActionResult> MyBids(int? pageNumber)
        {
            var cacheKey = "AllBids";
            var mybids = _cacheService.Retrieve<List<Bid>>(cacheKey);
            if (mybids == null)
            {
                var bidQuery = _bidsService.GetAll();
                if (bidQuery == null)
                {
                    return NotFound();
                }
                mybids = bidQuery.ToList();
                _cacheService.Store(cacheKey, mybids, TimeSpan.FromMinutes(120));
            }
            return View(_listingHelper.CreatePaginatedList(mybids.Where(l => l.IdentityUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList(), pageNumber ?? 1, PageSize));
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cacheKey = $"Listing-{id}";
            try
            {
                var listing = _cacheService.Retrieve<Listing>(cacheKey);
                if (listing == null)
                {
                    listing = await _listingHelper.GetListingById(id);
                    _cacheService.Store(cacheKey, listing, TimeSpan.FromMinutes(120));
                }

                if (listing == null)
                {
                    return NotFound();
                }

                return View(listing);
            }
            catch (NullReferenceException ex)
            {
                // Gérer l'exception ici
                return BadRequest($"Une erreur s'est produite lors de la récupération des détails de la liste.{ex.Message}");
            }
        }

        // GET: Listings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ListingVM listingVM)
        {
            if (ModelState.IsValid)
            {
                if (listingVM.Image == null)
                {
                    return BadRequest();
                }
                var identityUser = await _userManager.GetUserAsync(User);
                var listing = await _listingHelper.CreateListing(listingVM, identityUser);
                if (listing != null)
                {
                    await _listingsService.Add(listing);
                    await _listingsService.SaveChanges(); // Add this line to save changes asynchronously
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(listingVM);
        }
        [HttpPost]
        public async Task<ActionResult> AddBid(BidViewModel bidViewModel)
        {
            if (ModelState.IsValid)
            {
                var cacheKey = $"Listing-{bidViewModel.ListingId}";
                try
                {
                    var listing = await _listingHelper.GetListingById(bidViewModel.ListingId);

                    if (listing == null)
                    {
                        return NotFound();
                    }

                    if (!BidValidator.IsValidBid(bidViewModel, listing))
                    {
                        // Ajoute une erreur au ModelState
                        ModelState.AddModelError("Price", "Votre mise doit être supérieure au prix annoncé et à la mise la plus récente.");

                        // Renvoie la vue avec le modèle contenant les erreurs
                        return View("Details", listing);
                    }
                    var identityUser = await _userManager.GetUserAsync(User);
                    var bidObj = new Bid
                    {
                        User = identityUser,
                        IdentityUserId = identityUser?.Id,
                        Price = bidViewModel.Price,
                        ListingId = bidViewModel.ListingId
                    };

                    await _bidsService.Add(bidObj);
                    listing.Price = bidViewModel.Price;
                    await _listingsService.SaveChanges();
                    _cacheService.Store(cacheKey, listing, TimeSpan.FromMinutes(120));

                }
                catch (NullReferenceException ex)
                {
                    // Gérer l'exception ici
                    return BadRequest($"Une erreur s'est produite lors de l'ajout de l'offre.{ex.Message}");
                }
            }

            var updatedListing = await _listingHelper.GetListingById(bidViewModel.ListingId);
            return View("Details", updatedListing);
        }
        public async Task<ActionResult> CloseBidding(int id)
        {
            var listing = new Listing();
            try
            {
                listing = await _listingHelper.GetListingById(id);
                listing.IsSold = true;
                await _listingsService.SaveChanges();
            }
            catch (NullReferenceException ex)
            {
                return BadRequest($"Une erreur s'est produite lors de la clôture des enchères.: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Une erreur inattendue s'est produite : {ex.Message}");
            }
            return View("Details", listing);
        }

        [HttpPost]
        public async Task<ActionResult> AddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    comment.IdentityUserId = _userManager.GetUserIdAsync(User);
                    comment.ListingId = comment.Id;
                    await _commentsService.Add(comment);
                }
                catch (NullReferenceException ex)
                {
                    return BadRequest($"Une erreur s'est produite lors de l'exécution de l'opération : {ex.Message}");
                }
            }
            var cacheKey = $"Listing-{comment.ListingId}";
            var listing = _cacheService.Retrieve<Listing>(cacheKey);
            listing ??= await _listingHelper.GetListingById(comment.ListingId);

            return View("Details", listing);
        }

    }
}
