using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IJwtService
{
    AccessTokenResult CreateAccessToken(Guid userId);
}