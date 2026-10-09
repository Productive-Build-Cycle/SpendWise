using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IIdentityService
{
    Task<IdentityOperationResult> CreateUserAsync(Guid userId, string userName, string password, CancellationToken cancellationToken = default);
}