// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Auctions.Areas.Identity.Pages.Account;

public class LoginModel : LoginBaseModel
{

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
         : base(signInManager, logger)
    {
    }
}
