namespace SpendWise.Infrastructure.Authentication.RefreshTokens;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "Authentication:RefreshToken";

    public int LifetimeDays { get; set; } = 7;

    public TimeSpan Lifetime => TimeSpan.FromDays(LifetimeDays);

    public void Validate()
    {
        if (LifetimeDays is < 1 or > 7)
        {
            throw new InvalidOperationException("Refresh Token lifetime must be between 1 and 7 days.");
        }
    }
}