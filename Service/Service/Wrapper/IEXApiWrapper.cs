// <copyright file="IEXApiWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using System.Diagnostics.CodeAnalysis;
using IEXSharp;
using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using IEXSharp.Model.Shared.Response;

[ExcludeFromCodeCoverage]
public class IEXApiWrapper : IIEXApiWrapper
{
    /// <inheritdoc />
    public async Task<IEXResponse<IEnumerable<HistoricalPriceResponse>>> GetHistoricalPricesAsync(string pubToken,
        string secToken, string ticker, ChartRange range,
        bool signRequest = false, bool useSandBox = false)
    {
        using var iexCloudClient = new IEXCloudClient(pubToken, secToken, false, false);
        return await iexCloudClient.StockPrices.HistoricalPriceAsync(ticker, range);
    }

    /// <inheritdoc />
    public async Task<IEXResponse<Quote>> GetRealTimeStockQuote(string pubToken,
        string secToken, string ticker)
    {
        using var iexCloudClient = new IEXCloudClient(pubToken, secToken, false, false);
        return await iexCloudClient.StockPrices.QuoteAsync(ticker);
    }
}