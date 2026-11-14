using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Application.Common.Behaviors;

public sealed class DomainExceptionTranslationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainValidationException ex)
        {
            throw new DomainRuleViolationException(ex.Message, ex);
        }
        catch (DomainConflictException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        catch (DomainException ex)
        {
            throw new DomainOperationException(ex);
        }
    }
}