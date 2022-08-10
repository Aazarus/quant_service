﻿// <copyright file="AlphaVantageWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

public class AlphaVantageWrapper : IAlphaVantageWrapper
{
    /// <summary>
    ///     AlphaVantage API url.
    /// </summary>
    private const string AlphaVantageUrl = "https://www.alphavantage.co/";

    /// <summary>
    ///     The HttpClient Factory.
    /// </summary>
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<AlphaVantageWrapper> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaVantageWrapper" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="clientFactory">The HttpClient Factory.</param>
    public AlphaVantageWrapper(ILogger<AlphaVantageWrapper> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    /// <inheritdoc />
    public async Task<string> GetStockEOD(string ticker, string start, string period, string apiKey)
    {
        var startDate = DateTime.Parse(start);
        string size = GetSize(startDate);
        string timeSeries = GetTimeSeries(period);
        string url = GenerateUrl(timeSeries, ticker, size, apiKey);
        var history = string.Empty;
        var client = _clientFactory.CreateClient("AlphaVantage");

        try
        {
            history = await client.GetStringAsync(url);
        }
        catch (Exception)
        {
            // ToDo: Log error when adding 3rd party logger.
            _logger.LogError($"Unknown error occurred calling AlphaVantage endpoint for ticker {ticker}.");
        }

        return history;
    }

    private static string GetSize(DateTime startDate)
    {
        return startDate < DateTime.Today.AddDays(-120) ? "full" : "compact";
    }

    private static string GetTimeSeries(string period)
    {
        if (period == null) throw new ArgumentNullException(nameof(period));

        return period.ToLowerInvariant() switch
        {
            "weekly" => "TIME_SERIES_WEEKLY_ADJUSTED",
            "monthly" => "TIME_SERIES_MONTHLY_ADJUSTED",
            _ => "TIME_SERIES_DAILY_ADJUSTED"
        };
    }

    private static string GenerateUrl(string timeSeries, string ticker, string size, string apiKey)
    {
        return
            $"{AlphaVantageUrl}query?function={timeSeries}&symbol={ticker}&outputsize={size}&apikey={apiKey}&datatype=csv";
    }
}