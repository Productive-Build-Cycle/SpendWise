using MediatR;

namespace SpendWise.Application.Features.Commands.Accounts.Delete;

public sealed record DeleteAccountCommand(Guid AccountId) : IRequest<Unit>;