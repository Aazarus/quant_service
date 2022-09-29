// <copyright file="AlphaVantageMarketDataValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.AlphaVantage;
using Moq;
using Service.Controllers;
using Service.Services;
using Xunit;

public class AlphaVantageMarketDataValuesControllerTests
{
    private readonly Mock<IAlphaVantageService> _avService;
    private readonly Mock<ILogger<AlphaVantageMarketDataValuesController>> _logger;
    private AlphaVantageMarketDataValuesController _controller = null!;

    public AlphaVantageMarketDataValuesControllerTests()
    {
        _logger = new Mock<ILogger<AlphaVantageMarketDataValuesController>>();
        _avService = new Mock<IAlphaVantageService>();
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod(null!, "", "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("", "", "", string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod(" ", "", "", string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNullStartDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", null!, "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForEmptyStartDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "", "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForWhitespaceStartDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", " ", "", string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNonDateStartDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "test date", "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForInvalidDateStartDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2020/01/01", "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNullEndDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", null!, "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("End Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForEmptyEndDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", "", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("End Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForWhitespaceEndDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-02-02", " ", string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("End Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNonDateEndDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", "test date", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("End Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForInvalidDateEndDate()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", "2020/01/01", "");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("End Date is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForNullPeriod()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", "2022-02-01", null!);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForEmptyPeriod()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-01-01", "2022-02-01", string.Empty);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnBadRequestForWhitespacePeriod()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockEod("IBM", "2022-02-02", "2022-02-01", " ");

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnNotFoundForUnknownTicker()
    {
        // Arrange
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockEOD(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<StockData>());

        // Act
        var actual = await _controller.GetAvStockEod(ticker, "2022-02-02", "2022-02-01", "weekly");

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be($"No data for Ticker: {ticker}");
    }

    [Fact]
    public async Task GetAvStockEod_ShouldReturnOkWithCollectionForValidRequest()
    {
        // Arrange
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockEOD(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.AvEODData.ToList());

        // Act
        var actual = await _controller.GetAvStockEod(ticker, "2022-02-02", "2022-02-01", "weekly");

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(TestData.AvEODData.ToList());
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockBar(null!, 1, 1);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockBar(string.Empty, 1, 1);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockBar(" ", 1, 1);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnBadRequestForInvalidInterval()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockBar("IBM", 0, 1);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("interval is invalid. Must be greater than 0.");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnBadRequestForInvalidOutputSize()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvStockBar("IBM", 1, 0);

        // Assert
        actual.Should().BeOfType(typeof(BadRequestObjectResult));

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("outputSize is invalid. Must be greater than 0.");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnNotFoundForUnknownTicker()
    {
        // Arrange
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockBar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<StockData>());

        // Act
        var actual = await _controller.GetAvStockBar("IBM", 1, 1);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be($"No data for Ticker: {ticker}");
    }

    [Fact]
    public async Task GetAvStockBar_ShouldReturnOkWithCollectionForValidRequest()
    {
        // Arrange
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(TestData.AvBarData.ToList());

        // Act
        var actual = await _controller.GetAvStockBar(ticker, 1, 1);

        // Assert
        actual.Should().BeOfType(typeof(OkObjectResult));

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj.Value.Should().BeEquivalentTo(TestData.AvBarData.ToList());
    }

    [Fact]
    public async Task GetAvQuote_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvQuote(null!);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvQuote_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        var ticker = string.Empty;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvQuote_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        const string ticker = " ";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvQuote_ShouldReturnNotFoundForWhitespaceTicker()
    {
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockQuote(ticker))
            .ReturnsAsync(new AvStockQuote());

        // Act
        var actual = await _controller.GetAvQuote(ticker);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be($"No data for Ticker: {ticker}");
    }

    [Fact]
    public async Task GetAvQuote_ShouldReturnAnOkActionResult()
    {
        // Arrange
        const string ticker = "IBM";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetStockQuote(ticker))
            .ReturnsAsync(TestData.AvStockQuote);

        // Act
        var actual = await _controller.GetAvQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<OkObjectResult>();

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj!.Value.Should().BeEquivalentTo(TestData.AvStockQuote);
    }
}