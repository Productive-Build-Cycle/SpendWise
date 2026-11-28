using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Interfaces
{
    public interface IUserLocalDateToUtcConverter
    {
        DateTime ConvertLocalDateToUtc(
            DateOnly localDate,
            string timeZoneId);
    }
}
