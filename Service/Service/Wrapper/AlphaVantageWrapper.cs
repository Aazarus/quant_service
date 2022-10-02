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
        string timeSeries = GetStockEODTimeSeries(period);
        string url = GenerateEODUrl(timeSeries, ticker, size, apiKey);

        return await CallAlphaVantage(ticker, url);
    }

    /// <inheritdoc />
    public async Task<string> GetStockBar(string ticker, int interval, int outputSize, string apiKey)
    {
        string size = GetBarSize(outputSize);
        const string timeSeries = "TIME_SERIES_INTRADAY";
        string url =
            $"{AlphaVantageUrl}query?function={timeSeries}&symbol={ticker}&outputSize={size}&apikey={apiKey}&datatype=csv&interval={interval}min";

        return await CallAlphaVantage(ticker, url);
    }

    /// <inheritdoc />
    public async Task<string> GetStockQuote(string ticker, string apiKey)
    {
        const string function = "GLOBAL_QUOTE";
        string url = $"{AlphaVantageUrl}query?function={function}&symbol={ticker}&apikey={apiKey}&datatype=csv";

        return await CallAlphaVantage(ticker, url);
    }

    public async Task<string> GetFxEOD(string ticker, string start, string period, string apiKey)
    {
        // ToDo: This needs tests.
        ticker = SanitiseFxTicker(ticker);
        string fromTicker = ticker[..3];
        string toTicker = ticker.Substring(3, 3);
        var startDate = DateTime.Parse(start);
        string function = GetFxEODFunction(period);
        string size = function == "FX_DAILY" ? GetEODSize(startDate) : "";

        string url = GenerateFxEODUrl(function, fromTicker, toTicker, size, apiKey);

        return await CallAlphaVantage(ticker, url);
    }

    private static string GetEODSize(DateTime startDate)
    {
        const int earlier = -1;
        const int outputSizeThreshold = -100;
        var dateThreshold = DateTime.Now.AddDays(outputSizeThreshold);
        return DateTime.Compare(startDate, dateThreshold) == earlier ? "compact" : "full";
    }

    private static string GetStockEODTimeSeries(string period)
    {
        if (period == null) throw new ArgumentNullException(nameof(period));

        return period.ToLowerInvariant() switch
        {
            "weekly" => "TIME_SERIES_WEEKLY_ADJUSTED",
            "monthly" => "TIME_SERIES_MONTHLY_ADJUSTED",
            _ => "TIME_SERIES_DAILY_ADJUSTED"
        };
    }

    private static string GetFxEODFunction(string period)
    {
        if (period == null) throw new ArgumentNullException(nameof(period), "Period is invalid.");

        return period.ToLowerInvariant() switch
        {
            "weekly" => "FX_WEEKLY",
            "monthly" => "FX_MONTHLY",
            _ => "FX_DAILY"
        };
    }

    private static string GenerateEODUrl(string timeSeries, string ticker, string size, string apiKey)
    {
        return
            $"{AlphaVantageUrl}query?function={timeSeries}&symbol={ticker}&outputsize={size}&apikey={apiKey}&datatype=csv";
    }

    private static string GenerateFxEODUrl(string function, string fromTicker, string toTicker, string size,
        string apiKey)
    {
        var outputSize = $"&outputsize={size}";
        if (string.IsNullOrWhiteSpace(size)) outputSize = string.Empty;
        return
            $"{AlphaVantageUrl}query?function={function}&from_symbol={fromTicker}&to_ticker={toTicker}{outputSize}&apikey={apiKey}&datatype=csv";
    }

    private static string GetBarSize(int outputSize)
    {
        return outputSize > 100 ? "full" : "compact";
    }

    private async Task<string> CallAlphaVantage(string ticker, string url)
    {
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

    private static string SanitiseFxTicker(string ticker)
    {
        ticker = ticker.Contains('/') ? ticker.Replace("/", string.Empty) : ticker;

        if (ticker.Length != 6) throw new Exception("Confirm ticker is in the correct format e.g. GBPUSD or GBP/USD");

        return ticker;
    }
}