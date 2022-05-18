// <copyright file="StockValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Service.Controllers;
using Xunit;

public class StockValuesControllerTests
{
    private readonly Mock<ILogger<StockValuesController>> _logger;
    private StockValuesController _controller = null!;
    private IEnumerable<IndexData>? _indexData;
    private Mock<QuantDataContext>? _mockDbContext;
    private IEnumerable<Price>? _prices;
    private IEnumerable<Symbol>? _symbols;

    public StockValuesControllerTests()
    {
        SetMockData();
        SetupDatabaseMocks();
        _logger = new Mock<ILogger<StockValuesController>>();
    }

    [Fact]
    public void ConfirmDataCheckedAndReadyLogged()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);

        // Act
        // Assert
        _logger.Verify(logger => logger.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == "Data checked and ready."),
            null,
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public void ConfirmExceptionThrownIfNoSymbolsAvailable()
    {
        // Arrange
        DbSet<Symbol>? symbols = null;
        _mockDbContext = new Mock<QuantDataContext>();
        _mockDbContext.Setup(x => x.Symbols).Returns(symbols);

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object)
        );

        // Assert
        _logger.Verify(logger => logger.Log(
            LogLevel.Critical,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == "No Symbols available."),
            It.IsAny<InvalidOperationException>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public void ConfirmExceptionThrownIfNoPricesAvailable()
    {
        // Arrange
        var mockSymbols = CreateDbSetMock(_symbols!);
        DbSet<Price>? prices = null;
        _mockDbContext = new Mock<QuantDataContext>();
        _mockDbContext.Setup(x => x.Symbols).Returns(mockSymbols.Object);
        _mockDbContext.Setup(x => x.Prices).Returns(prices);

        // Act
        Assert.Throws<InvalidOperationException>(() =>
            _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object)
        );

        // Assert
        _logger.Verify(logger => logger.Log(
            LogLevel.Critical,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == "No Prices available."),
            It.IsAny<InvalidOperationException>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public void GetSymbols_ShouldReturnTheFullCollection()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);

        // Act
        var actual = _controller.GetSymbols();

        // Assert
        actual.Should().BeEquivalentTo(_symbols);
    }

    [Fact]
    public void GetSymbol_ShouldReturnASingleSymbolForAValidId()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        int id = TestData.Symbols.FirstOrDefault()!.SymbolId;

        // Act
        var actual = _controller.GetSymbol(id);

        // Assert
        actual.Should().BeEquivalentTo(TestData.Symbols.FirstOrDefault()!);
    }

    [Fact]
    public void GetSymbol_ShouldReturnNullForAnInvalidId()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const int id = -1;

        // Act
        var actual = _controller.GetSymbol(id);

        // Assert
        actual.Should().BeNull();
    }

    private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
    {
        var elementsAsQueryable = elements.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

        return dbSetMock;
    }

    private void SetMockData()
    {
        _symbols = TestData.Symbols;
        _indexData = TestData.IndexData;
        _prices = TestData.Prices;
    }

    private void SetupDatabaseMocks()
    {
        var mockSymbols = CreateDbSetMock(_symbols!);
        var mockIndexData = CreateDbSetMock(_indexData!);
        var mockPrices = CreateDbSetMock(_prices!);
        _mockDbContext = new Mock<QuantDataContext>();
        _mockDbContext.Setup(x => x.Symbols).Returns(mockSymbols.Object);
        _mockDbContext.Setup(x => x.IndexData).Returns(mockIndexData.Object);
        _mockDbContext.Setup(x => x.Prices).Returns(mockPrices.Object);
    }
}