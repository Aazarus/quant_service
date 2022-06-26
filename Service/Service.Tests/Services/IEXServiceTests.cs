// <copyright file="IEXServiceTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Service.Services;
using Wrapper;
using Xunit;

public class IEXServiceTests
{
    private readonly ApiKeySettings.IEX _apiKey;
    private readonly Mock<IIEXApiWrapper> _apiWrapper;
    private readonly Mock<ILogger<YahooService>> _logger;
    private IEXService? _service;

    public IEXServiceTests()
    {
        _logger = new Mock<ILogger<YahooService>>();
        _apiWrapper = new Mock<IIEXApiWrapper>();
        _apiKey = new ApiKeySettings.IEX
        {
            PublishableToken = "pub_token",
            SecurityToken = "sec_token"
        };
    }

    [Fact]
    public async Task GetStock_ShouldReturnAnEmptyCollectionIfNullReturnedFromApi()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetHistoricalPricesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChartRange>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(await Task.FromResult<IEXResponse<IEnumerable<HistoricalPriceResponse>>>(null!));

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStock("IBM", ChartRange.SixMonths);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, type) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
    }

    [Fact]
    public async Task GetStock_ShouldReturnAnEmptyCollectionWithALogEntryForARequestWithAnErrorMessage()
    {
        // Arrange
        const string errorMessage = "An Error Occurred.";
        _apiWrapper.Setup(w =>
                w.GetHistoricalPricesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChartRange>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(await Task.FromResult(
                new IEXResponse<IEnumerable<HistoricalPriceResponse>>
                {
                    ErrorMessage = errorMessage
                }));

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStock("IBM", ChartRange.SixMonths);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStock_ShouldReturnAValidCollectionForAValidRequest()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetHistoricalPricesAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChartRange>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(await Task.FromResult(GetHistoricalPrice()));

        var expected = TestData.StockDataDaily.ToList();

        // IEX doesn't provide Adjusted Close
        expected.ForEach(e => e.CloseAdj = default);

        // IEX doesn't provide time for DateTime
        expected.ForEach(e => e.Date = e.Date.Date);

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStock("IBM", ChartRange.SixMonths);

        // Assert
        actual.Should().NotBeNull();
        actual.Count.Should().Be(expected.Count);
        actual.Should().BeEquivalentTo(expected);

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, type) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
    }

    private static IEXResponse<IEnumerable<HistoricalPriceResponse>> GetHistoricalPrice()
    {
        return new IEXResponse<IEnumerable<HistoricalPriceResponse>>
        {
            Data = new List<HistoricalPriceResponse>
            {
                new()
                {
                    date = DateTime.Now.AddDays(-365).ToShortDateString(),
                    open = 123.321m,
                    high = 124.321m,
                    low = 122.021m,
                    close = 124.320m,
                    volume = 32425284
                },
                new()
                {
                    date = DateTime.Now.AddDays(-364).ToShortDateString(),
                    open = 124.320m,
                    high = 125.81m,
                    low = 122.021m,
                    close = 125.30m,
                    volume = 32443284
                },
                new()
                {
                    date = DateTime.Now.AddDays(-363).ToShortDateString(),
                    open = 125.30m,
                    high = 125.481m,
                    low = 123.021m,
                    close = 124.30m,
                    volume = 31425284
                },
                new()
                {
                    date = DateTime.Now.AddDays(-362).ToShortDateString(),
                    open = 124.30m,
                    high = 126.481m,
                    low = 122.021m,
                    close = 124.30m,
                    volume = 41425284
                }
            }
        };
    }
}