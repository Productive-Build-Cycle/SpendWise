using Microsoft.AspNetCore.Identity;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;

namespace SpendWise.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityOperationResult> CreateUserAsync(Guid userId, string userName, string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return IdentityOperationResult.Success();

        var errors = result.Errors.Select(error => new IdentityErrorModel(error.Code, error.Description)).ToArray();

        return IdentityOperationResult.Failure(errors);
    }

    public async Task<Guid?> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByNameAsync(userName);

        if (user is null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        return result.Succeeded ? user.Id : null;
    }
}