// <copyright file="AlphaVantageWrapper.cs" company="Sevna Software LTD">
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
    ///     The HttpClient.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<AlphaVantageWrapper> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaVantageWrapper" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="client">The HttpClient.</param>
    public AlphaVantageWrapper(ILogger<AlphaVantageWrapper> logger, HttpClient client)
    {
        _logger = logger;
        _httpClient = client;
    }

    /// <inheritdoc />
    public async Task<string> GetStockEOD(string ticker, string start, string period, string apiKey)
    {
        var startDate = DateTime.Parse(start);
        string size = GetEODSize(startDate);
        string timeSeries = GetEODTimeSeries(period);
        string url = GenerateEODUrl(timeSeries, ticker, size, apiKey);
        var history = string.Empty;

        try
        {
            history = await _httpClient.GetStringAsync(url);
        }
        catch (Exception)
        {
            _logger.LogError($"Unknown error occurred calling AlphaVantage endpoint for ticker {ticker}.");
        }

        return history;
    }

    /// <inheritdoc />
    public async Task<string> GetStockBar(string ticker, int interval, int outputSize, string apiKey)
    {
        string size = GetBarSize(outputSize);
        const string timeSeries = "TIME_SERIES_INTRADAY";
        string url =
            $"{AlphaVantageUrl}query?function={timeSeries}&symbol={ticker}&outputSize={size}&apikey={apiKey}&datatype=csv&interval={interval}min";
        var history = string.Empty;

        try
        {
            history = await _httpClient.GetStringAsync(url);
        }
        catch (Exception)
        {
            _logger.LogError($"Unknown error occurred calling AlphaVantage endpoint for ticker {ticker}.");
        }

        return history;
    }

    private static string GetEODSize(DateTime startDate)
    {
        return startDate < DateTime.Today.AddDays(-120) ? "full" : "compact";
    }

    private static string GetEODTimeSeries(string period)
    {
        if (period == null) throw new ArgumentNullException(nameof(period));

        return period.ToLowerInvariant() switch
        {
            "weekly" => "TIME_SERIES_WEEKLY_ADJUSTED",
            "monthly" => "TIME_SERIES_MONTHLY_ADJUSTED",
            _ => "TIME_SERIES_DAILY_ADJUSTED"
        };
    }

    private static string GenerateEODUrl(string timeSeries, string ticker, string size, string apiKey)
    {
        return
            $"{AlphaVantageUrl}query?function={timeSeries}&symbol={ticker}&outputsize={size}&apikey={apiKey}&datatype=csv";
    }

    private static string GetBarSize(int outputSize)
    {
        return outputSize > 100 ? "full" : "compact";
    }
}