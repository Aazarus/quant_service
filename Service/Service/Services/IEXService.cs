// <copyright file="IEXService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using IEXSharp.Helper;
using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using Models;
using Wrapper;

public class IEXService : IIEXService
{
    /// <summary>
    ///     The IEX Api Wrapper.
    /// </summary>
    private readonly IIEXApiWrapper _apiWrapper;

    /// <summary>
    ///     The API Keys.
    /// </summary>
    private readonly ApiKeySettings.IEX _iexKey;

    /// <summary>
    ///     The Application Logger.
    /// </summary>
    private readonly ILogger<YahooService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IEXService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="iexKey">The IEX Api Keys.</param>
    /// <param name="apiWrapper">The IEX Api Wrapper.</param>
    public IEXService(ILogger<YahooService> logger, ApiKeySettings.IEX iexKey, IIEXApiWrapper apiWrapper)
    {
        _logger = logger;
        _iexKey = iexKey;
        _apiWrapper = apiWrapper;
    }

    /// <summary>
    ///     Requests stock data for a given ticker from IEX.
    /// </summary>
    /// <param name="ticker">The stock ticker.</param>
    /// <param name="range">The Time range for the request.</param>
    /// <returns>A collection of StockData objects.</returns>
    public async Task<List<StockData>> GetStock(string ticker,
        ChartRange range)
    {
        var response =
            await _apiWrapper.GetHistoricalPricesAsync(_iexKey.PublishableToken, _iexKey.SecurityToken, ticker, range);
        return ProcessIEXResponseForStockData(ticker, response).OrderBy(d => d.Date).ToList();
    }

    /// <summary>
    ///     Process the IEX Response for getting Historical Prices and converts it to a collection of StockData objects.
    /// </summary>
    /// <param name="ticker">The Stock ticker.</param>
    /// <param name="response">The IEX Response.</param>
    /// <returns>A collection of StockData objects.</returns>
    private IEnumerable<StockData> ProcessIEXResponseForStockData(
        string ticker,
        IEXResponse<IEnumerable<HistoricalPriceResponse>> response)
    {
        var result = new List<StockData>();

        if (response != null)
        {
            if (response.ErrorMessage != null)
            {
                _logger.LogError(response.ErrorMessage);
            }
            else
            {
                if (response.Data != null)
                    result.AddRange(from data in response.Data
                        let dt = data.GetTimestampInUTC()
                        select new StockData
                        {
                            Ticker = ticker,
                            Date = dt.Date,
                            Open = data.open ?? 0,
                            High = data.high ?? 0,
                            Low = data.low ?? 0,
                            Close = data.close ?? 0,
                            Volume = data.volume ?? 0
                        });
            }
        }

        return result;
    }
}