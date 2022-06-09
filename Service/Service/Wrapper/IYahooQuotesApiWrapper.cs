// <copyright file="IYahooQuotesApiWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using NodaTime;
using YahooQuotesApi;

public interface IYahooQuotesApiWrapper
{
    /// <summary>
    ///     Gets a Security by its ticker and gets the Price History by the frequency given and starting at the given start
    ///     date.
    /// </summary>
    /// <param name="ticker">The ticker for the security.</param>
    /// <param name="start">The start date for the price history.</param>
    /// <param name="frequency">The frequency for the price ticks.</param>
    /// <returns>A collection of PriceTicks.</returns>
    Task<IEnumerable<PriceTick>?> GetSecurityPriceHistoryAsync(string ticker, Instant start, Frequency frequency);
}