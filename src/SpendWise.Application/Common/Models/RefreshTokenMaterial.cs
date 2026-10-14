namespace SpendWise.Application.Common.Models;

public sealed record RefreshTokenMaterial(string RawToken, string TokenHash)
{
    public override string ToString() => TokenHash;
}

