// <copyright file="StockValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

    [Fact]
    public void GetSymbolAndPrices_ShouldReturnASymbolWithPrices()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        int id = TestData.Symbols.FirstOrDefault()!.SymbolId;
        const string start = "2017-11-07";
        const string end = "2017-11-11";
        var expected = TestData.Symbols.FirstOrDefault()!;
        expected.Prices = TestData.Prices.Where(price => price.SymbolId == 1).ToList();

        // Act
        var actual = _controller.GetSymbolAndPrices(id, start, end);

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetSymbolAndPrices_ShouldReturnBadRequestForEmptyStart()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        int id = TestData.Symbols.FirstOrDefault()!.SymbolId;
        var start = string.Empty;
        const string end = "2017-11-11";

        // Act
        var actual = _controller.GetSymbolAndPrices(id, start, end);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Invalid argument provided");
    }

    [Fact]
    public void GetSymbolAndPrices_ShouldReturnBadRequestForEmptyEnd()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        int id = TestData.Symbols.FirstOrDefault()!.SymbolId;
        const string start = "2017-11-07";
        var end = string.Empty;

        // Act
        var actual = _controller.GetSymbolAndPrices(id, start, end);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Invalid argument provided");
    }

    [Fact]
    public void GetSymbolAndPrices_ShouldReturnNotFoundForUnknownId()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const int id = 1001;
        const string start = "2017-11-07";
        const string end = "2017-11-11";

        // Act
        var actual = _controller.GetSymbolAndPrices(id, start, end);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be("Symbol with id: '1001' not found.");
    }

    [Fact]
    public void GetSymbolAndPrices_ShouldReturnNotFoundForIdWithNoPrices()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const int id = 1;
        const string start = "2020-11-07";
        const string end = "2020-11-11";

        // Act
        var actual = _controller.GetSymbolAndPrices(id, start, end);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should()
            .Be("Symbol with id: '1' found but no Prices available for given start and end dates.");
    }

    [Fact]
    public void GetSymbolWithTicker_ShouldReturnASymbolUsingAValidTickerId()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var expected = TestData.Symbols.FirstOrDefault()!;

        // Act
        var actual = _controller.GetSymbolWithTicker(expected.Ticker!);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetSymbolWithTicker_ShouldReturnNullForAnUnknownTicker()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "abcdef";

        // Act
        var actual = _controller.GetSymbolWithTicker(ticker);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnASymbolWithPricesByTicker()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "IBM";
        const string start = "2017-11-07";
        const string end = "2017-11-11";
        var expected = TestData.Symbols.FirstOrDefault()!;
        expected.Prices = TestData.Prices.Where(price => price.SymbolId == 1).ToList();

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var ticker = string.Empty;
        const string start = "2017-11-05";
        const string end = "2017-11-11";

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Invalid argument provided");
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnBadRequestForEmptyStart()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "IBM";
        var start = string.Empty;
        const string end = "2017-11-11";

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Invalid argument provided");
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnBadRequestForEmptyEnd()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "IBM";
        const string start = "2017-11-05";
        var end = string.Empty;

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Invalid argument provided");
    }

    [Fact]
    public void CreateSymbol_ReturnsOKForValidNewSymbol()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var newTicker = new Symbol
        {
            Ticker = "ABC",
            Region = "US",
            Sector = "Information Technology"
        };

        // Act
        var actual = _controller.CreateSymbol(newTicker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(OkObjectResult));
        ((OkObjectResult) actual).StatusCode.Should().Be(200);
    }

    [Fact]
    public void CreateSymbol_ReturnsOKWithExistingSymbolIfPresent()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var expected = TestData.Symbols.First();
        var newTicker = new Symbol
        {
            Ticker = expected.Ticker
        };

        // Act
        var actual = _controller.CreateSymbol(newTicker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(OkObjectResult));
        var actualObj = actual as OkObjectResult;
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().Be(expected.SymbolId);
    }

    [Fact]
    public void CreateSymbol_InvalidModelReturnsBadRequest()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        _controller.ModelState.AddModelError("Error", "Error occurred");

        // Act
        var actual = _controller.CreateSymbol(new Symbol());

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        ((BadRequestObjectResult) actual).StatusCode.Should().Be(400);
    }

    [Fact]
    public void UpdateSymbol_UpdatesValidSymbol()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var expected = TestData.Symbols.First();
        expected.Region = "Uk";

        // Act
        var actual = _controller.UpdateSymbol(expected);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(OkObjectResult));
        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(expected.SymbolId);
    }

    [Fact]
    public void UpdateSymbol_UnknownSymbolReturnsBadRequest()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        var expected = new Symbol
        {
            Ticker = "abcde"
        };


        // Act
        var actual = _controller.UpdateSymbol(expected);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        ((BadRequestObjectResult) actual).StatusCode.Should().Be(400);
    }

    [Fact]
    public void UpdateSymbol_InvalidModelReturnsBadRequest()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        _controller.ModelState.AddModelError("Error", "Error occurred");

        // Act
        var actual = _controller.UpdateSymbol(new Symbol());

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType(typeof(BadRequestObjectResult));
        ((BadRequestObjectResult) actual).StatusCode.Should().Be(400);
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnNotFoundForUnknownId()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "abcde";
        const string start = "2017-11-07";
        const string end = "2017-11-11";

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be("Symbol with ticker: 'abcde' not found.");
    }

    [Fact]
    public void GetSymbolAndPriceWithTicker_ShouldReturnNotFoundForTickerWithNoPrices()
    {
        // Arrange
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
        const string ticker = "IBM";
        const string start = "2020-11-07";
        const string end = "2020-11-11";

        // Act
        var actual = _controller.GetSymbolAndPriceWithTicker(ticker, start, end);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should()
            .Be("Symbol with ticker: 'IBM' found but no Prices available for given start and end dates.");
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