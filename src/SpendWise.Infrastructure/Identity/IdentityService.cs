using Microsoft.AspNetCore.Identity;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;

namespace SpendWise.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityOperationResult> CreateUserAsync(Guid userId, string userName, string password,
        CancellationToken cancellationToken = default)
    {
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
}