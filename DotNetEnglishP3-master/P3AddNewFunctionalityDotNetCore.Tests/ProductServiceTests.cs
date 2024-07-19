using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private static ProductService SetupProductService()
        {
            var cart = Mock.Of<ICart>();
            var productRepository = Mock.Of<IProductRepository>();
            var orderRepository = Mock.Of<IOrderRepository>();
            var localizer = Mock.Of<IStringLocalizer<ProductService>>();
            // Set up the localizer to return the name of the LocalizedString
            Mock.Get(localizer).Setup(l => l[It.IsAny<string>()]).Returns((string name) => new LocalizedString(name, name));

            var productService = new ProductService(cart, productRepository, orderRepository, localizer);
            return productService;
        }


        [Fact]
        public void ModelErrors_NameIsString_NoMissingName()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Name = "Arthur" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().NotContain("MissingName");
        }

        [Fact]
        public void ModelErrors_NameIsNull_MissingName()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Name = null };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingName");
        }

        [Fact]
        public void ModelErrors_NameIsEmptyString_MissingName()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Name = "" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingName");
        }

        [Fact]
        public void ModelErrors_NameIsWhiteSpace_MissingName()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Name = "  " };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingName");
        }

        [Fact]
        public void ModelErrors_PriceIsDecimal_NoMissingPrice()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = "9.95" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().NotContain("MissingPrice");
        }

        [Fact]
        public void ModelErrors_PriceIsNull_MissingPrice()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = null };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingPrice");
        }

        [Fact]
        public void ModelErrors_PriceIsEmptyString_MissingPrice()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = "" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingPrice");
        }

        [Fact]
        public void ModelErrors_PriceIsWhiteSpace_MissingPrice()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = " " };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingPrice");
        }

        [Fact]
        public void ModelErrors_PriceIsLetters_PriceNotANumber()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = "abc" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("PriceNotANumber");
        }

        [Fact]
        public void ModelErrors_PriceIsZero_PriceNotGreaterThanZero()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = "0" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("PriceNotGreaterThanZero");
        }

        [Fact]
        public void ModelErrors_PriceIsNegative_PriceNotGreaterThanZero()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Price = "-1" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("PriceNotGreaterThanZero");
        }

        [Fact]
        public void ModelErrors_StockIsInteger_NoMissingQuantity()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "420" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().NotContain("MissingQuantity");
        }

        [Fact]
        public void ModelErrors_StockIsNull_MissingQuantity()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = null };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingQuantity");
        }

        [Fact]
        public void ModelErrors_StockIsEmptyString_MissingQuantity()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingQuantity");
        }

        [Fact]
        public void ModelErrors_StockIsWhiteSpace_MissingQuantity()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = " " };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("MissingQuantity");
        }

        [Fact]
        public void ModelErrors_StockIsLetters_StockNotAnInteger()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "abc" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("StockNotAnInteger");
        }

        [Fact]
        public void ModelErrors_StockIsDecimal_StockNotAnInteger()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "1.5" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("StockNotAnInteger");
        }

        [Fact]
        public void ModelErrors_StockIsZero_StockNotGreaterThanZero()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "0" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("StockNotGreaterThanZero");
        }

        [Fact]
        public void ModelErrors_StockIsNegative_StockNotGreaterThanZero()
        {
            // Arrange
            ProductService productService = SetupProductService();

            // Act
            var product = new ProductViewModel { Stock = "-1" };
            List<string> errors = productService.CheckProductModelErrors(product);

            // Assert
            errors.Should().Contain("StockNotGreaterThanZero");
        }

    }
}