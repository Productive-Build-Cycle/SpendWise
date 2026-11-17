using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;
using TransactionEntity = SpendWise.Domain.Entities.Transactions;

namespace SpendWise.Application.Features.Commands.Transactions.Create;

public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private const string SupportedCurrency = "USD";

    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var typeName = Enum.GetNames<TransactionTypes>()
            .FirstOrDefault(name =>
                string.Equals(name, request.TransactionType.Trim(), StringComparison.OrdinalIgnoreCase));

        if (typeName is null)
            throw new DomainValidationException("Invalid transaction type.");

        var transactionType = Enum.Parse<TransactionTypes>(typeName);

        Account? sourceAccount = null;
        if (request.SourceAccountId.HasValue)
        {
            sourceAccount = await _accountRepository.FindByIdForUserAsync(
                request.SourceAccountId.Value, userId, cancellationToken);

            if (sourceAccount is null)
                throw new NotFoundException("Source account not found.");
        }

        Account? destinationAccount = null;
        if (request.DestinationAccountId.HasValue)
        {
            destinationAccount = await _accountRepository.FindByIdForUserAsync(
                request.DestinationAccountId.Value, userId, cancellationToken);

            if (destinationAccount is null)
                throw new NotFoundException("Destination account not found.");
        }

        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.FindByIdForUserReadOnlyAsync(
                request.CategoryId.Value, userId, cancellationToken);

            if (category is null)
                throw new NotFoundException("Category not found.");

            // Income and expense categories are not interchangeable.
            if (transactionType == TransactionTypes.Income && category.CategoryType != CategoryType.Income ||
                transactionType == TransactionTypes.Expense && category.CategoryType != CategoryType.Expense)
                throw new DomainValidationException("Category type does not match the transaction type.");
        }

        var transaction = new TransactionEntity(
            request.SourceAccountId,
            request.DestinationAccountId,
            transactionType,
            userId,
            new Money(request.Amount, new Currency(SupportedCurrency)),
            request.Description,
            NormalizeToUtc(request.TransactionDate),
            request.CategoryId);

        // Balances change together with the new transaction in a single SaveChanges call.
        transaction.ApplyToAccounts(sourceAccount, destinationAccount);

        _transactionRepository.Add(transaction);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }

    private static DateTime NormalizeToUtc(DateTime? date)
    {
        if (!date.HasValue)
            return DateTime.UtcNow;

        return date.Value.Kind switch
        {
            DateTimeKind.Utc => date.Value,
            DateTimeKind.Local => date.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
        };
    }
}
