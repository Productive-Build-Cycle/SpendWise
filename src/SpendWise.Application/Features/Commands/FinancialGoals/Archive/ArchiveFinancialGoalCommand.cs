using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Archive
{
    public sealed record ArchiveFinancialGoalCommand(Guid FinancialGoalId) : IRequest<Unit>;

}
