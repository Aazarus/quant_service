// <copyright file="AlphaVantageWrapperTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Wrappers;

using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Wrapper;
using Xunit;

public class AlphaVantageWrapperTests
{
    private readonly Mock<ILogger<AlphaVantageWrapper>> _logger;
    private HttpClient? _httpClient;
    private AlphaVantageWrapper? _wrapper;

    public AlphaVantageWrapperTests()
    {
        _logger = new Mock<ILogger<AlphaVantageWrapper>>();
    }

    [Theory]
    [InlineData("IBM", true, "weekly", "api_key")]
    [InlineData("IBM", false, "monthly", "api_key")]
    [InlineData("IBM", true, "daily", "api_key")]
    public async Task GetStockEOD_ShouldReturnTheStringFromAlphaVantage(string ticker, bool full, string period,
        string apiKey)
    {
        // Arrange
        const string expected = "result from AV";
        string start = DateTime.Today.ToLongDateString();

        // Test Size
        if (full) start = DateTime.Today.AddDays(-121).ToLongDateString();

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetStockEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetStockEOD_ShouldThrowIfPeriodIsNull()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Today.ToLongDateString();
        string? period = null!;
        const string apiKey = "api_key";

        _httpClient = new HttpClient(new HttpClientHandler());
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        // Assert
        _ = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _wrapper.GetStockEOD(ticker, start, period, apiKey));
    }

    // This test will cover throwing the exception for all methods as they will use the same private method to make the call and throw if necessary
    [Fact]
    public async Task AnyCallToAlphaVantage_ShouldThrowExceptionIfRequestFailsWithDefaultMessage()
    {
        // Arrange
        const string ticker = "IBM";
        string start = DateTime.Today.ToLongDateString();
        const string period = "weekly";
        const string apiKey = "api_key";
        const string errorMessage = $"Unknown error occurred calling AlphaVantage endpoint for ticker {ticker}.";

        // Setup factory
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(null);


        _httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetStockEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().Be(string.Empty);
        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Theory]
    [InlineData("IBM", 1, 99, "api_key")]
    [InlineData("IBM", 1, 101, "api_key")]
    public async Task GetStockBar_ShouldReturnTheResultFromAlphaVantage(string ticker, int interval, int outputSize,
        string apiKey)
    {
        // Arrange
        const string expected = "result from AV";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetStockBar(ticker, interval, outputSize, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetStockBar_ShouldThrowExceptionIfRequestFailsWithDefaultMessage()
    {
        // Arrange
        const string ticker = "IBM";
        const int interval = 1;
        const int outputSize = 50;
        const string apiKey = "ApiKey";
        const string errorMessage = $"Unknown error occurred calling AlphaVantage endpoint for ticker {ticker}.";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(null);

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetStockBar(ticker, interval, outputSize, apiKey);

        // Assert
        actual.Should().Be(string.Empty);
        _logger.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString() == errorMessage),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }

    [Fact]
    public async Task GetStockQuote_ShouldReturnTheStringFromAlphaVantage()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "IBM";
        const string apiKey = "ApiKey";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetStockQuote(ticker, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldThrowExceptionForInvalidTicker()
    {
        // Arrange
        const string ticker = "GBPUSDEUR";
        var start = DateTime.Now.AddYears(-10).ToString(CultureInfo.InvariantCulture);
        const string period = "weekly";
        const string apiKey = "ApiKey";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
            await _wrapper.GetFxEOD(ticker, start, period, apiKey));
    }

    // This test handles checking the ticker sanitise method works.
    [Fact]
    public async Task GetAvFxEod_ShouldRemoveBackslashFromTicker()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        var start = DateTime.Now.AddYears(-10).ToString(CultureInfo.InvariantCulture);
        const string period = "weekly";
        const string apiKey = "ApiKey";
        const string expectedTickerToFrom = "from_symbol=GBP&to_symbol=USD";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedTickerToFrom)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldUseFullIfDateIsLessThan100DaysAndPeriodIsDailyFromCurrentDate()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.ToLongDateString();
        const string period = "daily";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "outputsize=full";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldUseCompactIfDateIsMoreThan100DaysAndPeriodIsDailyFromCurrentDate()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-101).ToLongDateString();
        const string period = "daily";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "outputsize=compact";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldNotUseOutputSizeForWeeklyPeriodFromCurrentDate()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-101).ToLongDateString();
        const string period = "weekly";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "&outputsize=";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    !rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldThrowExceptionForInvalidPeriod()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-121).ToLongDateString();
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "outputsize=compact";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _wrapper.GetFxEOD(ticker, start, null!, apiKey));
    }

    [Fact]
    public async Task GetAvFxEod_ShouldUseWeeklyStringForRespectivePeriod()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-121).ToLongDateString();
        const string period = "weekly";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "FX_WEEKLY";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldUseMonthlyStringForRespectivePeriod()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-121).ToLongDateString();
        const string period = "monthly";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "FX_MONTHLY";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldUseDailyStringForNonWeeklyOrMonthlyPeriod()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-121).ToLongDateString();
        const string period = "";
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "FX_DAILY";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxEod_ShouldReturnValidAVResult()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        string start = DateTime.Now.AddDays(-90).ToLongDateString();
        const string period = "";
        const string apiKey = "ApiKey";
        const string expectedUrl =
            "https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=GBP&to_symbol=USD&outputsize=full&apikey=ApiKey&datatype=csv";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedUrl)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxEOD(ticker, start, period, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxBar_ShouldUseFullWheOutputsizeGreaterThan100()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        const int interval = 5;
        const int outputsize = 101;
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "outputsize=full";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxBar(ticker, interval, outputsize, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxBar_ShouldUseCompactWheOutputsizeLessThan100()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        const int interval = 5;
        const int outputsize = 99;
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString = "outputsize=compact";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxBar(ticker, interval, outputsize, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task GetAvFxBar_ShouldSetTheExpectedUrlReturningExpectedResult()
    {
        // Arrange
        const string expected = "result from AV";
        const string ticker = "GBP/USD";
        const int interval = 1;
        const int outputsize = 99;
        const string apiKey = "ApiKey";
        const string expectedOutputsizeString =
            "https://www.alphavantage.co/query?function=FX_INTRADAY&from_symbol=GBP&to_symbol=USD&interval=1min&outputsize=compact&apikey=ApiKey&datatype=csv";

        // Setup HttpMessageHandler
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(rm =>
                    rm.RequestUri!.AbsoluteUri.Contains(expectedOutputsizeString)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expected)
            });

        _httpClient = new HttpClient(mockHttpMessageHandler.Object);
        _wrapper = new AlphaVantageWrapper(_logger.Object, _httpClient);

        // Act
        string actual = await _wrapper.GetFxBar(ticker, interval, outputsize, apiKey);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }
}