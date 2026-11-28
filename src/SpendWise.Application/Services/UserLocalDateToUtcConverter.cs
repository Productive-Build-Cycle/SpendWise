using SpendWise.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Services
{
    public class UserLocalDateToUtcConverter : IUserLocalDateToUtcConverter
    {
        public DateTime ConvertLocalDateToUtc(
            DateOnly localDate,
            string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localDateTime = localDate.ToDateTime(TimeOnly.MinValue);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
        }
    }
}
