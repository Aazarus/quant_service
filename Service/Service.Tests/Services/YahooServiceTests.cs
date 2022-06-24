// <copyright file="YahooServiceTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using NodaTime;
using Service.Services;
using Wrapper;
using Xunit;
using YahooQuotesApi;

public class YahooServiceTests
{
    private readonly Mock<IYahooQuotesApiWrapper> _apiWrapper;
    private readonly Mock<ILogger<YahooService>> _logger;
    private YahooService? _service;

    public YahooServiceTests()
    {
        _logger = new Mock<ILogger<YahooService>>();
        _apiWrapper = new Mock<IYahooQuotesApiWrapper>();
    }

    [Fact]
    public async Task GetStockDataWithPrices_ShouldThrowExceptionWhenSecurityResultFromYahooQuotesIsNull()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetSecurityPriceHistoryAsync("IBM", It.IsAny<Instant>(), It.IsAny<Frequency>()))
            .ReturnsAsync(await Task.FromResult((List<PriceTick>) null!));

        _service = new YahooService(_logger.Object, _apiWrapper.Object);
        const string ticker = "IBM";
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(30);
        const string period = "daily";

        // Act
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetStockDataWithPrices(ticker, start, end, period));

        // Assert
        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == "Failed to get ticker: 'IBM' with history"),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockDataWithPrices_ShouldReturnACollectionOfStockData()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetSecurityPriceHistoryAsync("IBM", It.IsAny<Instant>(), It.IsAny<Frequency>()))
            .ReturnsAsync(await Task.FromResult(TestData.SecurityPriceHistory));

        _service = new YahooService(_logger.Object, _apiWrapper.Object);
        const string ticker = "IBM";
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(30);
        const string period = "daily";

        var expected = ConvertPriceTickToStockDataCollection(ticker, TestData.SecurityPriceHistory);

        // Act
        var actual = await _service.GetStockDataWithPrices(ticker, start, end, period);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetFrequencyFromPeriod_ShouldReturnCorrectEnumForWeekly()
    {
        // Arrange
        const string period = "weekly";

        // Act
        var actual = YahooService.GetFrequencyFromPeriod(period);

        // Assert
        actual.Should().Be(Frequency.Weekly);
    }

    [Fact]
    public void GetFrequencyFromPeriod_ShouldReturnCorrectEnumForMonthly()
    {
        // Arrange
        const string period = "monthly";

        // Act
        var actual = YahooService.GetFrequencyFromPeriod(period);

        // Assert
        actual.Should().Be(Frequency.Monthly);
    }

    [Fact]
    public void GetFrequencyFromPeriod_ShouldReturnCorrectEnumForDaily()
    {
        // Arrange
        const string period = "daily";

        // Act
        var actual = YahooService.GetFrequencyFromPeriod(period);

        // Assert
        actual.Should().Be(Frequency.Daily);
    }

    private static IEnumerable<StockData> ConvertPriceTickToStockDataCollection(string ticker,
        IEnumerable<PriceTick> priceTicks)
    {
        return priceTicks.Select(priceTick => new StockData
            {
                Ticker = ticker,
                Date = priceTick.Date.ToDateTimeUnspecified(),
                Open = Convert.ToDecimal(priceTick.Open),
                High = Convert.ToDecimal(priceTick.High),
                Low = Convert.ToDecimal(priceTick.Low),
                Close = Convert.ToDecimal(priceTick.Close),
                CloseAdj = Convert.ToDecimal(priceTick.AdjustedClose),
                Volume = Convert.ToDecimal(priceTick.Volume)
            })
            .ToList();
    }
}