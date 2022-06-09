// <copyright file="YahooMarketDataValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Controllers;
using Service.Services;
using Xunit;

public class YahooMarketDataValuesControllerTests
{
    private readonly Mock<ILogger<YahooMarketDataValuesController>> _logger;
    private readonly Mock<IYahooService> _yahooService;
    private YahooMarketDataValuesController _controller = null!;

    public YahooMarketDataValuesControllerTests()
    {
        _logger = new Mock<ILogger<YahooMarketDataValuesController>>();
        _yahooService = new Mock<IYahooService>();
    }

    [Fact]
    public async Task GetYahooStock_ShouldReturnExceptionForNullTicker()
    {
        // Arrange
        _controller = new YahooMarketDataValuesController(_logger.Object, _yahooService.Object);
        string ticker = null!;
        const string start = "2015-01-01";
        const string end = "2015-02-01";
        const string period = "weekly";

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _controller.GetYahooStock(ticker, start, end, period)
        );
    }

    [Fact]
    public async Task GetYahooStock_ShouldReturnExceptionForNullStart()
    {
        // Arrange
        _controller = new YahooMarketDataValuesController(_logger.Object, _yahooService.Object);
        const string ticker = "ABC";
        string start = null!;
        const string end = "2015-02-01";
        const string period = "weekly";

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _controller.GetYahooStock(ticker, start, end, period)
        );
    }

    [Fact]
    public async Task GetYahooStock_ShouldReturnExceptionForNullEnd()
    {
        // Arrange
        _controller = new YahooMarketDataValuesController(_logger.Object, _yahooService.Object);
        const string ticker = "ABC";
        const string start = "2015-01-01";
        string end = null!;
        const string period = "weekly";

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _controller.GetYahooStock(ticker, start, end, period)
        );
    }

    [Fact]
    public async Task GetYahooStock_ShouldReturnExceptionForNullPeriod()
    {
        // Arrange
        _controller = new YahooMarketDataValuesController(_logger.Object, _yahooService.Object);
        const string ticker = "ABC";
        const string start = "2015-01-01";
        const string end = "2015-02-01";
        string period = null!;

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _controller.GetYahooStock(ticker, start, end, period)
        );
    }

    [Fact]
    public async Task GetYahooStock_ShouldReturnServiceCollectionForValidRequest()
    {
        // arrange
        _controller = new YahooMarketDataValuesController(_logger.Object, _yahooService.Object);
        const string ticker = "ABC";
        const string period = "daily";
        _yahooService.Setup(s =>
                s.GetStockDataWithPrices(ticker, It.IsAny<DateTime>(), It.IsAny<DateTime>(), period))
            .ReturnsAsync(await Task.FromResult(TestData.StockDataDaily));

        // Act
        var actual = await _controller.GetYahooStock(ticker, DateTime.Now.AddDays(-365).ToShortDateString(),
            DateTime.Now.AddDays(-1).ToShortDateString(), period);

        // Assert
        actual.Should().BeEquivalentTo(TestData.StockDataDaily);
    }
}