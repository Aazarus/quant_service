﻿// <copyright file="AlphaVantageServiceTests.cs" company="Sevna Software LTD">
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

        _apiWrapper.Setup(w => w.GetStockEOD(ticker, start, end, period))
            .ReturnsAsync(await Task.FromResult<string>(null!));

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(new List<StockData>());
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
            .ReturnsAsync(TestData.AvResponse);

        _service = new AlphaVantageService(_logger.Object, _apiKey, _apiWrapper.Object);

        // Act
        var actual = await _service.GetStockEOD(ticker, start, end, period);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(TestData.AvData);
    }
}