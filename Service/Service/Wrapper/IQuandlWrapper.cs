// <copyright file="IQuandlWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using Quandl.NET.Model.Response;

public interface IQuandlWrapper
{
    /// <summary>
    ///     Gets the stock data for a given ticker from Quandl.
    /// </summary>
    /// <param name="apiKey">The Quandl ApiKey.</param>
    /// <param name="ticker">The ticker.</param>
    /// <param name="start">The start date for the data.</param>
    /// <param name="end">The end date for the data.</param>
    /// <returns>TimeseriesDataResponse.</returns>
    Task<TimeseriesDataResponse> GetQuandlStock(string apiKey, string ticker, string start, string end);
}