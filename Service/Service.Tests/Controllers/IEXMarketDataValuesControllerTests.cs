// <copyright file="IEXMarketDataValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using IEXSharp.Model.CoreData.StockPrices.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.IEX;
using Moq;
using Service.Controllers;
using Service.Services;
using Xunit;

public class IEXMarketDataValuesControllerTests
{
    private readonly Mock<IIEXService> _iexService;
    private readonly Mock<ILogger<IEXMarketDataValuesController>> _logger;
    private IEXMarketDataValuesController _controller = null!;

    public IEXMarketDataValuesControllerTests()
    {
        _logger = new Mock<ILogger<IEXMarketDataValuesController>>();
        _iexService = new Mock<IIEXService>();
    }

    [Fact]
    public async Task GetIexStock_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexStock(null!);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetIexStock_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexStock(string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetIexStock_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexStock(" ");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetIexStock_ShouldReturnNotFoundForEmptyCollectionFromService()
    {
        // Arrange
        _iexService.Setup(s => s.GetStock(It.IsAny<string>(), It.IsAny<ChartRange>()))
            .ReturnsAsync(await Task.FromResult(new List<StockData>()));
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexStock("IBM");

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be("No data for Ticker: IBM");
    }

    [Fact]
    public async Task GetIexStock_ShouldReturnOKWithDataFromService()
    {
        // Arrange
        var expected = TestData.StockDataDaily.ToList();
        _iexService.Setup(s => s.GetStock(It.IsAny<string>(), It.IsAny<ChartRange>()))
            .ReturnsAsync(await Task.FromResult(expected));
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexStock("IBM");

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetIexQuote_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexQuote(null!);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetIexQuote_ShouldReturnNotFoundForNullResultFromService()
    {
        // Arrange
        _iexService.Setup(s => s.GetQuote(It.IsAny<string>()))
            .ReturnsAsync(await Task.FromResult<IexStockQuote>(null!));
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexQuote("IBM");

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be("No data for Ticker: IBM");
    }

    [Fact]
    public async Task GetIexQuote_ShouldReturnOKWithDataFromService()
    {
        // Arrange
        var expected = TestData.StockQuote;
        _iexService.Setup(s => s.GetQuote(It.IsAny<string>()))
            .ReturnsAsync(await Task.FromResult(expected));
        _controller = new IEXMarketDataValuesController(_logger.Object, _iexService.Object);

        // Act
        var actual = await _controller.GetIexQuote("IBM");

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(expected);
    }
}