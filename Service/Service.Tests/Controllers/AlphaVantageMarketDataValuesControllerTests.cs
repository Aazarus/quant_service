// <copyright file="AlphaVantageMarketDataValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System;
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
    public async Task GetAvStockEod_ShouldReturnNotFoundForTickerWithNoData()
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
    public async Task GetAvStockBar_ShouldReturnNotFoundForTickerWithNoData()
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
    public async Task GetAvQuote_ShouldReturnNotFoundForTickerWithNoData()
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

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(null!, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        var ticker = string.Empty;
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        const string ticker = " ";
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForNullStartDate()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, null!, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForEmptyStartDate()
    {
        const string ticker = "GBP/USD";
        var start = string.Empty;
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForWhitespaceStartDate()
    {
        const string ticker = "GBP/USD";
        const string start = " ";
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Start Date is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForNullPeriod()
    {
        // Arrange
        const string ticker = "GBPUSD";
        string start = DateTime.Now.ToLongDateString();
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, null!);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForEmptyPeriod()
    {
        // Arrange
        const string ticker = "GBPUSD";
        string start = DateTime.Now.ToLongDateString();
        var period = string.Empty;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnBadRequestForWhitespacePeriod()
    {
        // Arrange
        const string ticker = "GBPUSD";
        string start = DateTime.Now.ToLongDateString();
        const string period = " ";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Period is invalid");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnNotFoundForTickerWithNoData()
    {
        const string ticker = "GBPUSD";
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxEOD(ticker, start, period))
            .ReturnsAsync(new List<AvFxData>());

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be($"No data for FX Ticker: {ticker}");
    }

    [Fact]
    public async Task GetAvFxEOD_ShouldReturnAnOkActionResult()
    {
        const string ticker = "GBPUSD";
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxEOD(ticker, start, period))
            .ReturnsAsync(TestData.AvFxData);

        // Act
        var actual = await _controller.GetAvFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<OkObjectResult>();

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj!.Value.Should().BeEquivalentTo(TestData.AvFxData);
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnBadRequestForNullTicker()
    {
        // Arrange
        const int interval = 1;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxBar(null!, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnBadRequestForEmptyTicker()
    {
        // Arrange
        var ticker = string.Empty;
        const int interval = 1;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnBadRequestForWhitespaceTicker()
    {
        // Arrange
        const string ticker = " ";
        const int interval = 1;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Ticker is invalid");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(60)]
    public async Task GetFxBar_ShouldNotThrowBadRequestForValidIntervals(int interval)
    {
        // Arrange
        const string ticker = "GBPUSD";
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxBar(ticker, interval, outputsize))
            .ReturnsAsync(TestData.AvFxData);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetFxBar_ShouldThrowBadRequestForInvalidInterval()
    {
        // Arrange
        const string ticker = "GBPUSD";
        const int interval = 2;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxBar(ticker, interval, outputsize))
            .ReturnsAsync(TestData.AvFxData);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Interval must be 1, 5, 15, 30, or 60");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public async Task GetFxBar_ShouldThrowBadRequestForInvalidOutputsize(int outputsize)
    {
        // Arrange
        const string ticker = "GBPUSD";
        const int interval = 1;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxBar(ticker, interval, outputsize))
            .ReturnsAsync(TestData.AvFxData);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<BadRequestObjectResult>();

        var actualObj = actual as BadRequestObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(400);
        actualObj.Value.Should().Be("Outputsize must be greater than 0");
    }

    [Fact]
    public async Task GetAvFxBar_ShouldReturnNotFoundForTickerWithNoData()
    {
        const string ticker = "GBPUSD";
        const int interval = 1;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxBar(ticker, interval, outputsize))
            .ReturnsAsync(new List<AvFxData>());

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().BeOfType(typeof(NotFoundObjectResult));

        var actualObj = actual as NotFoundObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(404);
        actualObj.Value.Should().Be($"No data for FX Ticker: {ticker}");
    }

    [Fact]
    public async Task GetAvFxBar_ShouldReturnAnOkActionResult()
    {
        // Arrange
        const string ticker = "GBPUSD";
        const int interval = 1;
        const int outputsize = 99;
        _controller = new AlphaVantageMarketDataValuesController(_logger.Object, _avService.Object);

        _avService.Setup(s =>
                s.GetFxBar(ticker, interval, outputsize))
            .ReturnsAsync(TestData.AvFxData);

        // Act
        var actual = await _controller.GetAvFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeOfType<OkObjectResult>();

        var actualObj = actual as OkObjectResult;
        actualObj.Should().NotBeNull();
        actualObj!.StatusCode.Should().Be(200);
        actualObj!.Value.Should().BeEquivalentTo(TestData.AvFxData);
    }
}