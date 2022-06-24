// <copyright file="IIEXService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using IEXSharp.Model.CoreData.StockPrices.Request;
using Models;

public interface IIEXService
{
    /// <summary>
    ///     Gets the IEX Stock data for a given ticker within the provided range.
    /// </summary>
    /// <param name="ticker">The Stock Ticker.</param>
    /// <param name="range">The data range.</param>
    /// <returns>A collection of stock data.</returns>
    Task<List<StockData>> GetStock(string ticker,
        ChartRange range);
}