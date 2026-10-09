namespace SpendWise.Application.Common.Models;

public sealed record IdentityOperationResult(bool Succeeded, IReadOnlyCollection<IdentityErrorModel> Errors)
{
    public static IdentityOperationResult Success() => new(true, []);
    public static IdentityOperationResult Failure(IEnumerable<IdentityErrorModel> errors) => new(false, errors.ToArray());
}