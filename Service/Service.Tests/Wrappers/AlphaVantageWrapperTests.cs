// <copyright file="AlphaVantageWrapperTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Wrappers;

using System;
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

        // Setup factory
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

    [Fact]
    public async Task GetStockEOD_ShouldThrowExceptionIfRequestFailsWithDefaultMessage()
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
}