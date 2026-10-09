using MediatR;

namespace SpendWise.Application.Features.Commands.Authentication.Register;

public sealed record RegisterCommand(
    string UserName,
    string Password,
    string FirstName,
    string LastName) : IRequest<RegisterResponse>;