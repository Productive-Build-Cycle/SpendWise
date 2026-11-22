

namespace SpendWise.Application.Interfaces
{
    public interface IUserLocalDateProvider
    {
        DateOnly GetUserLocalDate(string timeZoneId);

        DateOnly GetUserLocalDate(DateTimeOffset dateTime, string timeZoneId);
    }
}
