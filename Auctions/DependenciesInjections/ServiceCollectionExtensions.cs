

using Auctions.Data;
using Auctions.Data.Services;
using Auctions.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auctions.DependenciesInjections;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, string connectionString, IWebHostEnvironment environment)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddMemoryCache();
        services.AddSignalR();

        if (environment.IsDevelopment())
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("AuctionsDatabase");
            });
        }
        else
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddControllersWithViews();
        services.AddTransient<ImageService>();
        services.AddScoped<IListingsService, ListingsService>();
        services.AddScoped<IBidsService, BidsService>();
        services.AddScoped<ICommentsService, CommentsService>();
        services.AddScoped<IListingHelper, ListingHelper>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IUserManager, UserManagerWrapper>();
        return services;
    }
}

