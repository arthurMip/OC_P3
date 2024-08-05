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
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests;

public class ProductServiceTests
{
    private P3Referential CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<P3Referential>()
            .UseSqlServer("Server=.;Database=P3Referential-Test;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        var context = new P3Referential(options, new ConfigurationBuilder().Build());
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        return context;
    }

    [Fact]
    public void ProductService_CreateProductInAdmin_ProductVisibleByUser()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new ProductRepository(context);
        var service = new ProductService(null, repository, null, null);

        // Act
        var newProduct1 = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        var newProduct2 = new ProductViewModel
        {
            Id = 2,
            Name = "Test2",
            Stock = "2",
            Price = "2"
        };
        service.SaveProduct(newProduct1);
        service.SaveProduct(newProduct2);
        ProductViewModel product1 = service.GetProductByIdViewModel(newProduct1.Id);
        ProductViewModel product2 = service.GetProductByIdViewModel(newProduct2.Id);
        List<ProductViewModel> products = service.GetAllProductsViewModel();

        // Assert
        product1.Should().NotBeNull()
            .And.BeEquivalentTo(newProduct1);

        product2.Should().NotBeNull()
            .And.BeEquivalentTo(newProduct2);

        products.Should().NotBeEmpty()
            .And.ContainEquivalentOf(newProduct1)
            .And.ContainEquivalentOf(product2);
    }

    [Fact]
    public void ProductService_DeleteProductInAdmin_ProductNotVisibleByUser()
    {
        // Arrange
        P3Referential context = CreateDbContext();
        var repository = new ProductRepository(context);
        var service = new ProductService(new Cart(), repository, null, null);

        // Act
        var newProduct1 = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        var newProduct2 = new ProductViewModel
        {
            Id = 2,
            Name = "Test2",
            Stock = "2",
            Price = "2"
        };
        service.SaveProduct(newProduct1);
        service.SaveProduct(newProduct2);
        service.DeleteProduct(newProduct1.Id);
        ProductViewModel product1 = service.GetProductByIdViewModel(newProduct1.Id);
        ProductViewModel product2 = service.GetProductByIdViewModel(newProduct2.Id);
        List<ProductViewModel> products = service.GetAllProductsViewModel();

        // Assert
        product1.Should().BeNull();

        product2.Should().NotBeNull()
            .And.BeEquivalentTo(newProduct2);

        products.Should().NotBeEmpty()
            .And.ContainEquivalentOf(product2)
            .And.NotContainEquivalentOf(newProduct1);
    }

    [Fact]
    public void ProductServie_AddProductsToCart_ProductsInCart()
    {
        // Arrange
        P3Referential context = CreateDbContext();
        var repository = new ProductRepository(context);
        var cart = new Cart();
        var service = new ProductService(cart, repository, null, null);

        // Act
        var newProduct1 = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        var newProduct2 = new ProductViewModel
        {
            Id = 2,
            Name = "Test2",
            Stock = "2",
            Price = "2"
        };
        service.SaveProduct(newProduct1);
        service.SaveProduct(newProduct2);

        Product p1 = service.GetProductById(1);
        Product p2 = service.GetProductById(2);

        cart.AddItem(p1, 1);
        cart.AddItem(p1, 1);
        cart.AddItem(p2, 1);

        // Assert
        cart.Lines.Should().HaveCount(2)
            .And.ContainEquivalentOf(new CartLine { Product = p1, Quantity = 2 })
            .And.ContainEquivalentOf(new CartLine { Product = p2, Quantity = 1 });
    }

    [Fact]
    public void ProductService_DeleteProductInAdmin_ProductNotInCart()
    {
        // Arrange
        P3Referential context = CreateDbContext();
        var repository = new ProductRepository(context);
        var cart = new Cart();
        var service = new ProductService(cart, repository, null, null);

        // Act
        var newProduct1 = new ProductViewModel
        {
            Id = 1,
            Name = "Test1",
            Stock = "1",
            Price = "1"
        };
        service.SaveProduct(newProduct1);
        Product p1 = service.GetProductById(1);
        cart.AddItem(p1 , 1);
        service.DeleteProduct(newProduct1.Id);

        // Assert
        cart.Lines.Should().BeEmpty();
    }
}
