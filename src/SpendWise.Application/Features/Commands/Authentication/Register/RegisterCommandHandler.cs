using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Commands.Authentication.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITransactionManager _transactionManager;

    public RegisterCommandHandler(IIdentityService identityService, IUserRepository userRepository,
        IApplicationDbContext dbContext, ITransactionManager transactionManager)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _dbContext = dbContext;
        _transactionManager = transactionManager;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();

        await _transactionManager.ExecuteAsync(
            async ct =>
            {
                var identityResult =
                    await _identityService.CreateUserAsync(userId, request.UserName, request.Password, ct);

                if (!identityResult.Succeeded)
                {
                    var failures = identityResult.Errors.Select(error =>
                        new ValidationFailure(GetPropertyName(error.Code), error.Description));
                    throw new ValidationException(failures);
                }

                var user = new User(userId, request.FirstName, request.LastName);

                _userRepository.Add(user);

                await _dbContext.SaveChangesAsync(ct);
            },
            cancellationToken);

        return new RegisterResponse(userId, request.UserName);
    }

    private static string GetPropertyName(string errorCode)
    {
        if (errorCode.Contains("Password", StringComparison.OrdinalIgnoreCase))
        {
            return nameof(RegisterCommand.Password);
        }

        if (errorCode.Contains("UserName", StringComparison.OrdinalIgnoreCase))
        {
            return nameof(RegisterCommand.UserName);
        }

        return string.Empty;
    }
}