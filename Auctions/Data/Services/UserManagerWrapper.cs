using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class UserManagerWrapper : IUserManager
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserManagerWrapper(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<IdentityUser> FindByIdAsync(string userId)
    {
        return _userManager.FindByIdAsync(userId);
    }


    public Task<IdentityUser> GetUserAsync(ClaimsPrincipal principal)
    {
        return _userManager.GetUserAsync(principal);
    }

    public string? GetUserIdAsync(ClaimsPrincipal principal)
    {
        return _userManager.GetUserId(principal);
    }
}