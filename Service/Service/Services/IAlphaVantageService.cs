// <copyright file="IAlphaVantageService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;

public interface IAlphaVantageService
{
    /// <summary>
    ///     Gets the End Of Day (EOD) data for a given ticker between the start and end dates for a given period of time.
    /// </summary>
    /// <param name="ticker">The Stock Ticker.</param>
    /// <param name="start">The start Date.</param>
    /// <param name="end">The end Date.</param>
    /// <param name="period">The period.</param>
    /// <returns>List of StockData.</returns>
    Task<List<StockData>> GetStockEOD(string ticker, string start, string end, string period);
}