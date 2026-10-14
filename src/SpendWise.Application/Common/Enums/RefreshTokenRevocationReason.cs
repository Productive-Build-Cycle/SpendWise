namespace SpendWise.Application.Common.Enums;

public enum RefreshTokenRevocationReason : byte
{
    Rotation = 1,
    Logout = 2,
    PasswordChanged = 3,
    ReuseDetected = 4,
    UserDisabled = 5,
    AdminRevoked = 6
}