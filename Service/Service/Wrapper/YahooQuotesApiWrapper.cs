// <copyright file="YahooQuotesApiWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using System.Diagnostics.CodeAnalysis;
using NodaTime;
using YahooQuotesApi;

/// <summary>
///     Provides a wrapper for the YahooQuotesApi library.
/// </summary>
[ExcludeFromCodeCoverage]
public class YahooQuotesApiWrapper : IYahooQuotesApiWrapper
{
    /// <inheritdoc />
    public async Task<IEnumerable<PriceTick>?> GetSecurityPriceHistoryAsync(string ticker, Instant start,
        Frequency frequency)
    {
        var yahooService = await CreateYahooQuotesWithStartAndFrequency(start, frequency)
            .GetAsync(ticker, HistoryFlags.PriceHistory);

        return yahooService?.PriceHistory.Value;
    }

    /// <summary>
    ///     Creates a YahooQuotes object to collect Price history from the given start date and for the given frequency.
    /// </summary>
    /// <param name="start">The start date for the price history.</param>
    /// <param name="frequency">The frequency for the price ticks.</param>
    /// <returns>A YahooQuotes object.</returns>
    private static YahooQuotes CreateYahooQuotesWithStartAndFrequency(Instant start, Frequency frequency)
    {
        return new YahooQuotesBuilder()
            .WithHistoryStartDate(start)
            .WithPriceHistoryFrequency(frequency)
            .Build();
    }
}