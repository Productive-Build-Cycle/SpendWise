using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IRefreshTokenService
{
    RefreshTokenMaterial Generate();

    string Hash(string rawToken);
}