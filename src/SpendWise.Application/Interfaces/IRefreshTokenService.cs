using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IRefreshTokenService
{
    TimeSpan Lifetime { get; }

    RefreshTokenMaterial Generate();

    string Hash(string rawToken);
}