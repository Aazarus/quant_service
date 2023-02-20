// <copyright file="IQuandlService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models.Quandl;

public interface IQuandlService
{
    /// <summary>
    ///     Gets the stock data for a given ticker.
    /// </summary>
    /// <param name="ticker">The ticker.</param>
    /// <param name="start">The start date for the data.</param>
    /// <param name="end">The end date for the data.</param>
    /// <returns>A collection of QuandlStockData items.</returns>
    Task<List<QuandlStockData>> GetQuandlStock(string ticker, string start, string end);
}