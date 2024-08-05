using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests;

public class ProductServiceTests : IDisposable
{
    private readonly P3Referential _context;
    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<P3Referential>()
            .UseSqlServer("Server=.;Database=P3Referential-Test;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;
        
        _context = new P3Referential(options, new ConfigurationBuilder().Build());
        _context.Database.EnsureDeleted();
        _context.Database.Migrate();
    }

    public void Dispose()
    {
        _context.RemoveRange(_context.Product);
        _context.SaveChanges();
    }

    [Fact]
    public void ProductService_CreateProductInAdmin_ProductVisibleByUser()
    {
        // Arrange
        var repository = new ProductRepository(_context);
        var service = new ProductService(null, repository, null, null);

        // Act
        var expectedProduct1 = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        var expectedProduct2 = new ProductViewModel
        {
            Id = 2,
            Name = "Test2",
            Stock = "2",
            Price = "2"
        };
        service.SaveProduct(expectedProduct1);
        service.SaveProduct(expectedProduct2);
        ProductViewModel resultProduct1 = service.GetProductByIdViewModel(expectedProduct1.Id);
        ProductViewModel resultproduct2 = service.GetProductByIdViewModel(expectedProduct2.Id);
        List<ProductViewModel> products = service.GetAllProductsViewModel();

        // Assert
        resultProduct1.Should().NotBeNull()
            .And.BeEquivalentTo(expectedProduct1);

        resultproduct2.Should().NotBeNull()
            .And.BeEquivalentTo(expectedProduct2);

        products.Should().HaveCount(2)
            .And.ContainEquivalentOf(expectedProduct1)
            .And.ContainEquivalentOf(expectedProduct2);
    }

    [Fact]
    public void ProductService_DeleteProductInAdmin_ProductNotVisibleByUser()
    {
        // Arrange
        var repository = new ProductRepository(_context);
        var service = new ProductService(new Cart(), repository, null, null);

        // Act
        var deletedProduct = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Description = "Test1",
            Stock = "1",
            Price = "1"
        };
        var expectedProduct = new ProductViewModel
        {
            Id = 2,
            Name = "Test2",
            Description = "Test2",
            Stock = "2",
            Price = "2"
        };
        service.SaveProduct(deletedProduct);
        service.SaveProduct(expectedProduct);
        service.DeleteProduct(deletedProduct.Id);
        ProductViewModel resultProduct1 = service.GetProductByIdViewModel(deletedProduct.Id);
        ProductViewModel resultProduct2 = service.GetProductByIdViewModel(expectedProduct.Id);
        List<ProductViewModel> products = service.GetAllProductsViewModel();

        // Assert
        resultProduct1.Should().BeNull();

        resultProduct2.Should().NotBeNull()
            .And.BeEquivalentTo(expectedProduct);

        products.Should().NotBeEmpty()
            .And.ContainEquivalentOf(expectedProduct)
            .And.NotContainEquivalentOf(deletedProduct);
    }

    [Fact]
    public void ProductService_DeleteProductInAdmin_ProductNotInCart()
    {
        // Arrange
        var repository = new ProductRepository(_context);
        var cart = new Cart();
        var service = new ProductService(cart, repository, null, null);

        // Act and Assert
        var deletedProduct = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        service.SaveProduct(deletedProduct);

        Product p1 = service.GetProductById(deletedProduct.Id);
        cart.AddItem(p1, 1);
        cart.Lines.Should().HaveCount(1);

        service.DeleteProduct(deletedProduct.Id);
        cart.Lines.Should().BeEmpty();
    }
}
