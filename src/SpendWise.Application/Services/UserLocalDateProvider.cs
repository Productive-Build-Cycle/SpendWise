using SpendWise.Application.Interfaces;


namespace SpendWise.Application.Services
{
    public class UserLocalDateProvider : IUserLocalDateProvider
    {
        private readonly TimeProvider _timeProvider;
        public UserLocalDateProvider(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        public DateOnly GetUserLocalDate(string timeZoneId)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTimeOffset userLocalTime = TimeZoneInfo.ConvertTime(utcNow, userTimeZone);
            return DateOnly.FromDateTime(userLocalTime.DateTime);
        }
    }
}
