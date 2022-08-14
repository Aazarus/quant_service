// <copyright file="IAlphaVantageWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

public interface IAlphaVantageWrapper
{
    /// <summary>
    ///     Gets the End Of Day (EOD) data for a given ticker between the start and end dates for a given period of time.
    /// </summary>
    /// <param name="ticker">The ticker.</param>
    /// <param name="start">The start Date.</param>
    /// <param name="period">The period.</param>
    /// <param name="apiKey">The ApiKey for AlphaVantage.</param>
    /// <returns>A string containing EOD data.</returns>
    Task<string> GetStockEOD(string ticker, string start, string period, string apiKey);

    /// <summary>
    ///     Gets the historical (1-2 months) intraday stock bar date for a given ticker.
    /// </summary>
    /// <param name="ticker">The ticker.</param>
    /// <param name="interval">The time interval.</param>
    /// <param name="outputSize">Number of data points.</param>
    /// <param name="apiKey">The ApiKey for AlphaVantage.</param>
    /// <returns>A string containing Stock Bar data.</returns>
    Task<string> GetStockBar(string ticker, int interval, int outputSize, string apiKey);

    /// <summary>
    ///     Gets the latest quote for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker.</param>
    /// <param name="apiKey">The ApiKey for AlphaVantage.</param>
    /// <returns>A string containing the Stock Quote data.</returns>
    Task<string> GetStockQuote(string ticker, string apiKey);
}