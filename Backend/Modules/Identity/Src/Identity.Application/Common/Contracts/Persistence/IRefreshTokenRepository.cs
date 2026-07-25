namespace Identity.Application.Common.Contracts.Persistence;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    Task<List<RefreshToken>> GetActiveTokensAsync(string userId, CancellationToken ct = default);
    void Update(RefreshToken refreshToken);
    void Update(List<RefreshToken> refreshTokens);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
}