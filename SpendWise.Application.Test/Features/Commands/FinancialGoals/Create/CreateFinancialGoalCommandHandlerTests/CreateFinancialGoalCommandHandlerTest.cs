using Moq;
using SpendWise.Application.Features.Commands.FinancialGoals.Create;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using Xunit;


namespace SpendWise.Application.Test.Features.Commands.FinancialGoals.Create.CreateFinancialGoalCommandHandlerTests
{
    public class CreateFinancialGoalCommandHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IUserLocalDateProvider> _userLocalDateProviderMock = new();
        private readonly Mock<IApplicationDbContext> _dbContextMock = new();
        private readonly Mock<IFinancialGoalRepository> _financialGoalRepositoryMock = new Mock<IFinancialGoalRepository>();

        private readonly CreateFinancialGoalCommandHandler _handler;

        public CreateFinancialGoalCommandHandlerTest()
        {
            _handler = new CreateFinancialGoalCommandHandler
                (
                 _userRepositoryMock.Object,
                 _currentUserMock.Object,
                 _userLocalDateProviderMock.Object,
                 _dbContextMock.Object,
                 _financialGoalRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_ValidCommand_ReturnFinancialGoalId()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User(userId, "Zahra", "Havaei");
            var currentDate = new DateOnly(2026, 11, 15);
            var targetDate = new DateOnly(2027, 11, 15);
            var command = new CreateFinancialGoalCommand("Buy a car",
                20000m, "USD", targetDate);

            _currentUserMock.Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(
                userId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userLocalDateProviderMock.Setup(x => x.GetUserLocalDate(user.TimeZoneId))
                .Returns(currentDate);
            _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            Assert.NotEqual(Guid.Empty, result);
        }

    }
}
