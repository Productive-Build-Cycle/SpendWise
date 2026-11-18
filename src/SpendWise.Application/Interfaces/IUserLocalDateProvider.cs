

namespace SpendWise.Application.Interfaces
{
    public interface IUserLocalDateProvider
    {
        DateOnly GetUserLocalDate(string timeZoneId);
    }
}
