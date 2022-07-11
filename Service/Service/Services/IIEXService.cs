// <copyright file="IIEXService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using IEXSharp.Model.CoreData.StockPrices.Request;
using Models;
using Models.IEX;

public interface IIEXService
{
    /// <summary>
    ///     Gets the IEX Stock data for a given ticker within the provided range.
    /// </summary>
    /// <param name="ticker">The Stock Ticker.</param>
    /// <param name="range">The Date Range.</param>
    /// <returns>A collection of StockData.</returns>
    Task<List<StockData>> GetStock(string ticker,
        ChartRange range);

    /// <summary>
    ///     Gets the Real-Time quote for a given ticker.
    /// </summary>
    /// <param name="ticker">The Ticker.</param>
    /// <returns>An IexStockQuote.</returns>
    Task<IexStockQuote> GetQuote(string ticker);
}