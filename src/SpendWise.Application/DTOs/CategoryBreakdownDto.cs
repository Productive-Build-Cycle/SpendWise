using SpendWise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.DTOs
{
    public sealed record CategoryBreakdownDto
    (
         Guid CategoryId,
         string CategoryName,
         TransactionTypes TransactionType,
         decimal TotalAmount
    );
}
