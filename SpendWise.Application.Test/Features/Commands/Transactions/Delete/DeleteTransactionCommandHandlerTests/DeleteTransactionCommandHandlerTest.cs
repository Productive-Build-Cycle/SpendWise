using Moq;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Features.Commands.Transactions.Delete;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;
using Xunit;
using TransactionEntity = SpendWise.Domain.Entities.Transactions;

namespace SpendWise.Application.Test.Features.Commands.Transactions.Delete.DeleteTransactionCommandHandlerTests
{
    public class DeleteTransactionCommandHandlerTest
    {
        private static readonly Currency Usd = new("USD");

        private readonly Mock<ITransactionRepository> _transactionRepositoryMock = new();
        private readonly Mock<IAccountRepository> _accountRepositoryMock = new();
        private readonly Mock<IApplicationDbContext> _dbContextMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();

        private readonly Guid _userId = Guid.NewGuid();
        private readonly DeleteTransactionCommandHandler _handler;

        public DeleteTransactionCommandHandlerTest()
        {
            _currentUserMock.Setup(x => x.UserId).Returns(_userId);
            _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _handler = new DeleteTransactionCommandHandler(
                _transactionRepositoryMock.Object,
                _accountRepositoryMock.Object,
                _dbContextMock.Object,
                _currentUserMock.Object);
        }

        private Account CreateAccount(decimal balance)
        {
            return new Account(_userId, "Test account", AccountType.Chequing, new Money(balance, Usd), null);
        }

        private TransactionEntity CreateExpense(Account source, decimal amount)
        {
            return new TransactionEntity(
                source.Id,
                null,
                TransactionTypes.Expense,
                _userId,
                new Money(amount, Usd),
                "Groceries",
                DateTime.UtcNow,
                Guid.NewGuid());
        }

        private void SetupTransaction(TransactionEntity? transaction, Guid id)
        {
            _transactionRepositoryMock
                .Setup(x => x.FindByIdForUserAsync(id, _userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);
        }

        private void SetupAccount(Account account)
        {
            _accountRepositoryMock
                .Setup(x => x.FindByIdForUserAsync(account.Id, _userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
        }

        [Fact]
        public async Task Handle_ExistingExpense_RestoresBalanceAndRemovesTransaction()
        {
            var source = CreateAccount(100);
            var transaction = CreateExpense(source, 40);
            transaction.ApplyToAccounts(source, null);
            SetupTransaction(transaction, transaction.Id);
            SetupAccount(source);

            await _handler.Handle(new DeleteTransactionCommand(transaction.Id), CancellationToken.None);

            Assert.Equal(100, source.Balance.Amount);
            _transactionRepositoryMock.Verify(x => x.Remove(transaction), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_TransactionNotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();
            SetupTransaction(null, id);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteTransactionCommand(id), CancellationToken.None));

            _transactionRepositoryMock.Verify(x => x.Remove(It.IsAny<TransactionEntity>()), Times.Never);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SourceAccountNotFound_ThrowsNotFoundException()
        {
            var source = CreateAccount(100);
            var transaction = CreateExpense(source, 40);
            SetupTransaction(transaction, transaction.Id);
            _accountRepositoryMock
                .Setup(x => x.FindByIdForUserAsync(source.Id, _userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteTransactionCommand(transaction.Id), CancellationToken.None));

            _transactionRepositoryMock.Verify(x => x.Remove(It.IsAny<TransactionEntity>()), Times.Never);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReversalFails_DoesNotRemoveOrSave()
        {
            var destination = CreateAccount(0);
            var income = new TransactionEntity(
                null,
                destination.Id,
                TransactionTypes.Income,
                _userId,
                new Money(100, Usd),
                "Salary",
                DateTime.UtcNow,
                Guid.NewGuid());
            income.ApplyToAccounts(null, destination);
            destination.Withdraw(new Money(90, Usd));
            SetupTransaction(income, income.Id);
            SetupAccount(destination);

            await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(new DeleteTransactionCommand(income.Id), CancellationToken.None));

            _transactionRepositoryMock.Verify(x => x.Remove(It.IsAny<TransactionEntity>()), Times.Never);
            _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
