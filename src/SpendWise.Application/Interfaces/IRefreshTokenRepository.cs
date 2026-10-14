using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IRefreshTokenRepository
{
    void Add(RefreshTokenRecord refreshToken);

    Task<RefreshTokenRecord?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}