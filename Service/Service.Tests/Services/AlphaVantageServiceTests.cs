// <copyright file="AlphaVantageServiceTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Models;
using Models.AlphaVantage;
using Moq;
using Service.Services;
using Wrapper;
using Xunit;

public class AlphaVantageServiceTests
{
    private readonly ApiKeySettings.AlphaVantage _apiKey;
    private readonly Mock<IAlphaVantageWrapper> _apiWrapper;
    private readonly Mock<ILogger<AlphaVantageService>> _logger;
    private AlphaVantageService? _service;

    public AlphaVantageServiceTests()
    {
        _logger = new Mock<ILogger<AlphaVantageService>>();
        _apiWrapper = new Mock<IAlphaVantageWrapper>();
        _apiKey = new ApiKeySettings.AlphaVantage
        {
            ApiKey = "api_key"
        };
    }

    [Fact]
    public async Task GetStockEOD_ShouldReturnAnEmptyCollectionForANullResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockEOD_ShouldReturnAnEmptyCollectionForAnEmptyResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockEOD_ShouldReturnAnEmptyCollectionForAWhitespaceResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockEOD_ShouldThrowANotSupportedExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync("This is a premium endpoint");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetStockEOD(ticker, start, end, period));
    }

    [Fact]
    public async Task GetStockEOD_ShouldThrowAnExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Error calling AlphaVantage. API key may be invalid.";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync("parameter apikey is invalid or missing");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetStockEOD(ticker, start, end, period));

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockEOD_ShouldReturnAValidCollectionOfStockData()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(TestData.AvEODResponse);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvEODData);
    }

    [Fact]
    public async Task GetStockEOD_ShouldThrowExceptionAndLogForInvalidResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-100).ToLongDateString();
        string end = DateTime.Now.ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "String '' was not recognized as a valid DateTime.";
        const string response = @"timestamp,open,high,low,close,adjusted close,volume,dividend amount
,";

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(response);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

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
    public async Task GetStockBar_ShouldReturnAnEmptyCollectionForANullResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockBar(ticker, 1, 1);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockBar_ShouldReturnAnEmptyCollectionForAnEmptyResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockBar(ticker, 1, 1);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockBar_ShouldReturnAnEmptyCollectionForAWhitespaceResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockBar(ticker, 1, 1);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockBar_ShouldThrowANotSupportedExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync("This is a premium endpoint");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetStockBar(ticker, 1, 1));
    }

    [Fact]
    public async Task GetStockBar_ShouldThrowAnExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Error calling AlphaVantage. API key may be invalid.";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync("parameter apikey is invalid or missing");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetStockBar(ticker, 1, 1));

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockBar_ShouldReturnAValidCollectionOfStockData()
    {
        // Arrange
        const string ticker = "IBM";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync(TestData.AvBarResponse);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockBar(ticker, 60, 100);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvBarData);
    }

    [Fact]
    public async Task GetStockBar_ShouldThrowExceptionAndLogForInvalidResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "String '' was not recognized as a valid DateTime.";
        const string response = @"timestamp,open,high,low,close,volume
,";

        _apiWrapper.Setup(w => w.GetStockBar(ticker, It.IsAny<int>(), It.IsAny<int>(), _apiKey.ApiKey))
            .ReturnsAsync(response);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockBar(ticker, 60, 100);

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
    public async Task GetStockQuote_ShouldReturnAnEmptyAvStockQuoteForNullResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new AvStockQuote());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldReturnAnEmptyAvStockQuoteForEmptyResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new AvStockQuote());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldReturnAnAvStockQuoteForAWhitespaceResponse()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Response is invalid (either null, empty, or whitespace)";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new AvStockQuote());

        _logger.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldThrowANotSupportedExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync("This is a premium endpoint");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetStockQuote(ticker));
    }

    [Fact]
    public async Task GetStockQuote_ShouldThrowAnExceptionIfRequired()
    {
        // Arrange
        const string ticker = "IBM";
        const string errorMessage = "Error calling AlphaVantage. API key may be invalid.";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync("parameter apikey is invalid or missing");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetStockQuote(ticker));

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldThrowAnExceptionAndLogForInvalidResponse()
    {
        // Arrange
        const string response = @"symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent
IBM,test";

        const string ticker = "IBM";
        const string errorMessage = "Input string was not in a correct format.";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync(response);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new AvStockQuote());

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldReturnAValidAvStockQuoteForValidResponse()
    {
        // Arrange
        const string ticker = "IBM";

        _apiWrapper.Setup(w => w.GetStockQuote(ticker, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(TestData.AvQuoteResponse));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockQuote(ticker);

        // Assert
        actual.Should().NotBeNull();
        actual.Ticker.Should().Be("IBM");
        actual.TimeStamp.Should().BeWithin(TimeSpan.FromSeconds(1));
        actual.Open.Should().Be(132.6200m);
        actual.High.Should().Be(134.0900m);
        actual.Low.Should().Be(131.9800m);
        actual.Price.Should().Be(134.0100m);
        actual.Volume.Should().Be(2767054m);
        actual.PrevClose.Should().Be(132.5400m);
        actual.Change.Should().Be(1.4700m);
        actual.ChangePercent.Should().Be(0.011091m);
    }

    [Fact]
    public async Task GetFxEOD_ShouldReturnEmptyAvFxDataCollectionForNullResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxEOD_ShouldReturnEmptyAvFxDataCollectionForEmptyResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxEOD_ShouldReturnEmptyAvFxDataCollectionForWhitespaceResponse()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxEOD_ShouldThrowANotSupportedExceptionIfPremiumAlphaVantageEndpoint()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync("This is a premium endpoint");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetFxEOD(ticker, start, period));
    }

    [Fact]
    public async Task GetFxEOD_ShouldThrowExceptionForInvalidAPIKey()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Error calling AlphaVantage. API key may be invalid.";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync("parameter apikey is invalid or missing");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetFxEOD(ticker, start, period));

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetFxEOD_ShouldThrowAnExceptionAndLogForInvalidResponse()
    {
        // Arrange
        const string response = @"timestamp,open,high,low,close,volume
2022-09-01 12:00:00,test";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";
        const string errorMessage = "Input string was not in a correct format.";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(response);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetFxEOD_ShouldReturnAValidCollectionOfAvfXDataForValidResponse()
    {
        // Arrange
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-60).ToLongDateString();
        const string period = "weekly";

        _apiWrapper.Setup(w => w.GetFxEOD(ticker, start, period, _apiKey.ApiKey))
            .ReturnsAsync(TestData.AvFxResponse);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxEOD(ticker, start, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvFxData);
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnEmptyAvFxDataCollectionForNullResponse()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnEmptyAvFxDataCollectionForEmptyResponse()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxBar_ShouldReturnEmptyAvFxDataCollectionForWhitespaceResponse()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvFxData>());
    }

    [Fact]
    public async Task GetFxBar_ShouldThrowANotSupportedExceptionIfPremiumAlphaVantageEndpoint()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync("This is a premium endpoint");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetFxBar(ticker, interval, outputsize));
    }

    [Fact]
    public async Task GetFxBar_ShouldThrowExceptionForInvalidAPIKey()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;
        const string errorMessage = "Error calling AlphaVantage. API key may be invalid.";

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync("parameter apikey is invalid or missing");

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetFxBar(ticker, interval, outputsize));

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetFxBar_ShouldThrowAnExceptionAndLogForInvalidResponse()
    {
        // Arrange
        const string response = @"timestamp,open,high,low,close,volume
2022-09-01 12:00:00,test";
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;
        const string errorMessage = "Input string was not in a correct format.";

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync(response);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await _service.GetFxBar(ticker, interval, outputsize);

        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetBar_ShouldReturnAValidCollectionOfAvfXDataForValidResponse()
    {
        // Arrange
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;

        _apiWrapper.Setup(w => w.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey))
            .ReturnsAsync(TestData.AvFxResponse);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetFxBar(ticker, interval, outputsize);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvFxData);
    }

    [Fact]
    public async Task GetSectorPref_ShouldReturnEmptyAvSectorPrefCollectionForNullResponse()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetSectorPref();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvSectorPref>());
    }

    [Fact]
    public async Task GetSectorPref_ShouldReturnEmptyAvSectorPrefCollectionForEmptyResponse()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(string.Empty));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetSectorPref();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvSectorPref>());
    }

    [Fact]
    public async Task GetSectorPref_ShouldReturnEmptyAvSectorPrefCollectionForWhitespaceResponse()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(" "));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetSectorPref();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<AvSectorPref>());
    }

    [Fact]
    public async Task GetSectorPref_ShouldThrowANotSupportedExceptionIfPremiumAlphaVantageEndpoint()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult("This is a premium endpoint"));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await _service.GetSectorPref());
    }

    [Fact]
    public async Task GetSectorPref_ShouldThrowExceptionForInvalidAPIKey()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult("parameter apikey is invalid or missing"));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _service.GetSectorPref());
    }

    [Fact]
    public async Task GetSectorPref_ShouldReturnAValidCollectionOfAvSectorPrefForValidResponse()
    {
        // Arrange
        _apiWrapper.Setup(w => w.GetSectorPref(_apiKey.ApiKey))
            .ReturnsAsync(await Task.FromResult(TestData.AvSectorPrefResponse));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetSectorPref();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvSectorPrefsData);
    }
}