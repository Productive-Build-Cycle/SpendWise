using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Tests
{
    public class CurrencyTests
    {
        [Fact]
        public void Constructor_ValidCode_ConvertCodeToUpperCase()
        {
            //Arrange
            var currency = new Currency("cad");
            //Act
            var result = currency.Code;
            //Assert
            Assert.Equal("CAD", result);
        }

        [Fact]
        public void Equals_SameCurrencyCode_ReturnsTrue()
        {
            //Arrange
            var currency1 = new Currency("USD");
            var currency2 = new Currency("USD");
            //Act
            var result = currency1.Equals(currency2);
            //Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_DifferentCurrencyCode_ReturnsFalse()
        {
            //Arrange
            var currency1 = new Currency("USD");
            var currency2 = new Currency("CAD");
            //Act
            var result = currency1.Equals(currency2);
            //Assert
            Assert.False(result);
        }
    }
}
