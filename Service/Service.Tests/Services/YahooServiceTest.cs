// <copyright file="YahooServiceTest.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Service.Services;
using Wrapper;
using Xunit;
using YahooQuotesApi;

public class YahooServiceTest
{
    private readonly Mock<IYahooQuotesApiWrapper> _apiWrapper;
    private readonly Mock<ILogger<YahooService>> _logger;
    private YahooService? _service;

    public YahooServiceTest()
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

        // Act
        await _service.GetStockDataWithPrices(ticker, start, end, period);

        // Assert
        // ToDo: Confirm the result
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
}