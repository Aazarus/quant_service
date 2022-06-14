// <copyright file="YahooService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;
using NodaTime;
using NodaTime.Extensions;
using Wrapper;
using YahooQuotesApi;

/// <summary>
///     Handles data retrieval from Yahoo.
/// </summary>
public class YahooService : IYahooService
{
    /// <summary>
    ///     The YahooQuotesApi Wrapper.
    /// </summary>
    private readonly IYahooQuotesApiWrapper _apiWrapper;

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<YahooService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="YahooService" /> class.
    /// </summary>
    /// <param name="logger">The Application logger.</param>
    /// <param name="apiWrapper">The YahooQuotesApi wrapper.</param>
    public YahooService(ILogger<YahooService> logger, IYahooQuotesApiWrapper apiWrapper)
    {
        _logger = logger;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StockData>> GetStockDataWithPrices(string ticker, DateTime start, DateTime end,
        string period)
    {
        var frequency = GetFrequencyFromPeriod(period);
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
        var endDate = end.ToLocalDateTime().Date;

        var priceHistory =
            await _apiWrapper.GetSecurityPriceHistoryAsync(ticker, Instant.FromDateTimeUtc(start), frequency);
        if (priceHistory != null)
            return GetStockDataFromSecurity(ticker, priceHistory, endDate);

        _logger.LogInformation($"Failed to get ticker: '{ticker}' with history");
        throw new Exception($"Failed to get ticker: '{ticker}' with history");
    }

    /// <summary>
    ///     Matches a string to a YahooQuotesApi Frequency.
    /// </summary>
    /// <param name="period">The requested period.</param>
    /// <returns>A Frequency enum value.</returns>
    public static Frequency GetFrequencyFromPeriod(string period)
    {
        return period.ToLower() switch
        {
            "weekly" => Frequency.Weekly,
            "monthly" => Frequency.Monthly,
            _ => Frequency.Daily
        };
    }

    /// <summary>
    ///     Gets the price data from a security up to the endDate.
    /// </summary>
    /// <param name="ticker">The ticker for the security.</param>
    /// <param name="priceHistory">The price history for the security.</param>
    /// <param name="endDate">The date of the final price to collect.</param>
    /// <returns>A collection of StockData objects.</returns>
    private static IEnumerable<StockData> GetStockDataFromSecurity(string ticker, IEnumerable<PriceTick> priceHistory,
        LocalDate endDate)
    {
        return priceHistory.TakeWhile(h => endDate >= h.Date)
            .Select(h => new StockData
            {
                Ticker = ticker,
                Date = h.Date.ToDateTimeUnspecified(),
                Open = Convert.ToDecimal(h.Open),
                High = Convert.ToDecimal(h.High),
                Low = Convert.ToDecimal(h.Low),
                Close = Convert.ToDecimal(h.Close),
                CloseAdj = Convert.ToDecimal(h.AdjustedClose),
                Volume = Convert.ToDecimal(h.Volume)
            }).ToList();
    }
}