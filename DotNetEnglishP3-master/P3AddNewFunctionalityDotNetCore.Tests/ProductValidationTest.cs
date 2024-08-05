using FluentAssertions;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductValidationTest
    {
        [Theory]
        [InlineData("Arthur", "9.95", "420")]
        public void ModelErrors_ValidProduct_NoErrors(string name, string price, string stock)
        {
            // Arrange
            var product = new ProductViewModel { Name = name, Price = price, Stock = stock };
            var context = new ValidationContext(product, null, null);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ModelErrors_NameIsNull_MissingName(string name)
        {
            // Arrange
            var product = new ProductViewModel { Name = name };
            var context = new ValidationContext(product, null, null) { MemberName = "Name" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Name, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("MissingName");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ModelErrors_PriceIsNull_MissingPrice(string price)
        {
            // Arrange
            var product = new ProductViewModel { Price = price };
            var context = new ValidationContext(product, null, null) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Price, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("MissingPrice");
        }

        [Theory]
        [InlineData("abc")]
        public void ModelErrors_PriceIsLetters_PriceNotANumber(string price)
        {
            // Arrange
            var product = new ProductViewModel { Price = price };
            var context = new ValidationContext(product, null, null) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Price, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("PriceNotANumber");
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        public void ModelErrors_PriceIsZero_PriceNotGreaterThanZero(string price)
        {
            // Arrange
            var product = new ProductViewModel { Price = price };
            var context = new ValidationContext(product, null, null) { MemberName = "Price" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Price, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("PriceNotGreaterThanZero");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ModelErrors_StockIsNull_MissingQuantity(string stock)
        {
            // Arrange
            var product = new ProductViewModel { Stock = stock };
            var context = new ValidationContext(product, null, null) { MemberName = "Stock" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Stock, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("MissingQuantity");
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("1.5")]
        public void ModelErrors_StockIsLetters_StockNotAnInteger(string stock)
        {
            // Arrange
            var product = new ProductViewModel { Stock = stock };
            var context = new ValidationContext(product, null, null) { MemberName = "Stock" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Stock, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("QuantityNotAnInteger");
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        public void ModelErrors_StockIsZero_StockNotGreaterThanZero(string stock)
        {
            // Arrange
            var product = new ProductViewModel { Stock = stock };
            var context = new ValidationContext(product, null, null) { MemberName = "Stock" };
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateProperty(product.Stock, context, results);

            // Assert
            results.Select(r => r.ErrorMessage).Should().Contain("StockNotGreaterThanZero");
        }
    }
}