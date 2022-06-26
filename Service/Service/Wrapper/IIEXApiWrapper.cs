// <copyright file="IIEXApiWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;

public interface IIEXApiWrapper
{
    /// <summary>
    ///     Gets Price data for a given ticker from IEX.
    /// </summary>
    /// <param name="pubToken">The Publishable Token for IEX API.</param>
    /// <param name="secToken">The Security Token for IEX API.</param>
    /// <param name="ticker">The stock ticker</param>
    /// <param name="range">The Chart range.</param>
    /// <param name="signRequest">Whether to sign the request.</param>
    /// <param name="useSandBox">Whether to use a Sandbox endpoint.</param>
    /// <returns>An IEXResponse with a collection of HistoricalPriceResponses.</returns>
    Task<IEXResponse<IEnumerable<HistoricalPriceResponse>>> GetHistoricalPricesAsync(string pubToken, string secToken,
        string ticker, ChartRange range, bool signRequest = false, bool useSandBox = false);
}