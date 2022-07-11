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
using IEXSharp.Model.Shared.Response;
using Microsoft.Extensions.Logging;
using Models;
using Models.IEX;
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
            It.IsAny<LogLevel>(),
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
            .ReturnsAsync(await Task.FromResult(TestData.IEXResponse));

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
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, type) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
    }

    [Fact]
    public async Task GetQuote_ShouldReturnANewIexStockQuoteIfNullReturnedFromApi()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetRealTimeStockQuote(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
            .ReturnsAsync(await Task.FromResult<IEXResponse<Quote>>(null!));

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetQuote("IBM");

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new IexStockQuote());

        _logger.Verify(log => log.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, type) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
    }

    [Fact]
    public async Task GetQuote_ShouldReturnANewIexStockQuoteWithALogEntryForARequestWithAnErrorMessage()
    {
        // Arrange
        const string errorMessage = "An Error Occurred.";
        _apiWrapper.Setup(w =>
                w.GetRealTimeStockQuote(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
            .ReturnsAsync(await Task.FromResult(
                new IEXResponse<Quote>
                {
                    ErrorMessage = errorMessage
                }));

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetQuote("IBM");

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new IexStockQuote());

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetQuote_ShouldReturnAValidIexStockQuoteForAValidRequest()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetRealTimeStockQuote(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
            .ReturnsAsync(await Task.FromResult(TestData.IEXQuote));

        var expected = TestData.StockQuote;

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetQuote("IBM");

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);

        _logger.Verify(log => log.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, type) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Never);
    }

    [Fact]
    public async Task GetQuote_ShouldThrowAnExceptionForInvalidUnixTime()
    {
        // Arrange
        _apiWrapper.Setup(w =>
                w.GetRealTimeStockQuote(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ))
            .ReturnsAsync(await Task.FromResult(TestData.IEXQuoteException));

        _service = new IEXService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        _ = Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.GetQuote("IBM"));
    }
}