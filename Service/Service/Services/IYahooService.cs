// <copyright file="IYahooService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;

public interface IYahooService
{
    /// <summary>
    ///     Returns a collection of start data for a given ticker and the period between a start and end date.
    /// </summary>
    /// <param name="ticker">The Ticker for the security.</param>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <param name="period">The time period for prices.</param>
    /// <returns>Collection of StockData.</returns>
    /// <exception cref="InvalidOperationException">Throws if fails to create YahooQuotes object.</exception>
    /// <exception cref="Exception">Throws if fails to find security with ticker.</exception>
    Task<IEnumerable<StockData>> GetStockDataWithPrices(string ticker, DateTime start, DateTime end, string period);
}