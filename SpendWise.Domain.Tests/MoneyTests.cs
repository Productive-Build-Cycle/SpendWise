using SpendWise.Domain.ValueObjects;
using SpendWise.Domain.Exceptions;


namespace SpendWise.Domain.Tests
{
    public class MoneyTests
    {
        [Fact]
        public void Money_ShouldThrowValidationException_WhenAmountIsNegative()
        {
            // Arrange
            decimal negativeAmount = -10;
            var currency = new Currency("USD");
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => new Money(negativeAmount, currency));
        }

        [Fact]
        public void Constructor_NullCurrency_ShouldThrowArgumentNullException()
        {
            // Arrange
            decimal amount = 10;
            Currency? currency = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Money(amount, currency!));
        }

        [Fact]
        public void Add_DifferentCurrencies_ThrowValidationExceptions()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(5, new Currency("IRR"));
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => money1.Add(money2));
        }

        [Fact]
        public void Subtract_DifferentCurrencies_ThrowValidationExceptions()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(5, new Currency("IRR"));
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => money1.Subtract(money2));
        }

        [Fact]
        public void Equals_SameAmountAndCurrency_ReturnsTrue()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(10, new Currency("USD"));
            // Act
            bool areEqual = money1.Equals(money2);
            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_DifferentAmount_ReturnsFalse()
        {
            //Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(5, new Currency("USD"));
            //Act
            bool areEqual = money1.Equals(money2);
            //Assert
            Assert.False(areEqual);
        }

        [Fact]
        public void Equals_DifferentCurrency_ReturnsFalse()
        {
            //Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(10, new Currency("IRR"));
            //Act
            bool areEqual = money1.Equals(money2);
            //Assert
            Assert.False(areEqual);
        }

        [Fact]
        public void GetHashCode_SameAmountAndCurrency_ReturnsSameHashCode()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(10, new Currency("USD"));
            // Act
            int hashCode1 = money1.GetHashCode();
            int hashCode2 = money2.GetHashCode();
            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void Add_SameCurrency_ReturnsCorrectAmount()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(5, new Currency("USD"));
            // Act
            var result = money1.Add(money2);
            // Assert
            Assert.Equal(15, result.Amount);
        }

        [Fact]
        public void Subtract_SameCurrency_ReturnsCorrectAmount()
        {
            // Arrange
            var money1 = new Money(10, new Currency("USD"));
            var money2 = new Money(5, new Currency("USD"));
            // Act
            var result = money1.Subtract(money2);
            // Assert
            Assert.Equal(5, result.Amount);
        }

        [Fact]
        public void Subtract_SecoundAmountGreaterThanFirst_ThrowsValidationException()
        {
            // Arrange
            var money1 = new Money(5, new Currency("USD"));
            var money2 = new Money(10, new Currency("USD"));
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => money1.Subtract(money2));
        }

        [Fact]
        public void Constructor_AmountWithMoreThanTwoDecimalPlaces_ThrowsValidationException()
        {
            var currency = new Currency("USD");

            Assert.Throws<DomainValidationException>(() => new Money(10.123m, currency));
        }

        [Fact]
        public void Constructor_AmountExceedsDatabasePrecision_ThrowsValidationException()
        {
            var currency = new Currency("USD");

            Assert.Throws<DomainValidationException>(() => new Money(10_000_000_000_000_000m, currency));
        }

        [Fact]
        public void Constructor_MaximumSupportedAmount_CreatesMoneySuccessfully()
        {
            var currency = new Currency("USD");

            var money = new Money(9_999_999_999_999_999.99m, currency);

            Assert.Equal(9_999_999_999_999_999.99m, money.Amount);
        }
    }
}