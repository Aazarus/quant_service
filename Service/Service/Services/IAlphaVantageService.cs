// <copyright file="IAlphaVantageService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;
using Models.AlphaVantage;

public interface IAlphaVantageService
{
    /// <summary>
    ///     Gets the End Of Day (EOD) data for a given ticker between the start and end dates for a given period of time.
    /// </summary>
    /// <param name="ticker">The stock Ticker.</param>
    /// <param name="start">The start Date.</param>
    /// <param name="end">The end Date.</param>
    /// <param name="period">The period.</param>
    /// <returns>List of StockData.</returns>
    Task<List<StockData>> GetStockEOD(string ticker, string start, string end, string period);

    /// <summary>
    ///     Get the Bar data for a given ticker with a set interval (time period) and output size.
    /// </summary>
    /// <param name="ticker">The stock Ticker.</param>
    /// <param name="interval">The time interval.</param>
    /// <param name="outputSize">The output size.</param>
    /// <returns>List of StockData.</returns>
    Task<List<StockData>> GetStockBar(string ticker, int interval, int outputSize);

    /// <summary>
    ///     Get the latest stock data for a given ticker.
    /// </summary>
    /// <param name="ticker">The stock Ticker.</param>
    /// <returns>An AvStockQuote object.</returns>
    Task<AvStockQuote> GetStockQuote(string ticker);
}