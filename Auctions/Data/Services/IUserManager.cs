using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public interface IUserManager
{
    Task<IdentityUser> FindByIdAsync(string userId);
    Task<IdentityUser> GetUserAsync(ClaimsPrincipal principal);
    string? GetUserIdAsync(ClaimsPrincipal principal);
}
