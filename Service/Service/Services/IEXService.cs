// <copyright file="IEXService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using IEXSharp.Helper;
using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Request;
using IEXSharp.Model.CoreData.StockPrices.Response;
using IEXSharp.Model.Shared.Response;
using Models;
using Models.IEX;
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
    private readonly ILogger<IEXService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IEXService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="iexKey">The IEX Api Keys.</param>
    /// <param name="apiWrapper">The IEX Api Wrapper.</param>
    public IEXService(ILogger<IEXService> logger, ApiKeySettings.IEX iexKey, IIEXApiWrapper apiWrapper)
    {
        _logger = logger;
        _iexKey = iexKey;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<List<StockData>> GetStock(string ticker,
        ChartRange range)
    {
        var response =
            await _apiWrapper.GetHistoricalPricesAsync(_iexKey.PublishableToken, _iexKey.SecurityToken, ticker, range);
        return ProcessIEXResponseForStockData(ticker, response).OrderBy(d => d.Date).ToList();
    }

    /// <inheritdoc />
    public async Task<IexStockQuote> GetQuote(string ticker)
    {
        var response = await _apiWrapper.GetRealTimeStockQuote(_iexKey.PublishableToken, _iexKey.SecurityToken, ticker);
        return ProcessIexResponseForRealTimeData(response);
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

    /// <summary>
    ///     Processes the IEX Response for getting the current price data for a ticker
    /// </summary>
    /// <param name="response">The response to process.</param>
    /// <returns>An IexStockQuote.</returns>
    private IexStockQuote ProcessIexResponseForRealTimeData(IEXResponse<Quote> response)
    {
        if (response != null)
        {
            if (response.ErrorMessage != null)
            {
                _logger.LogError(response.ErrorMessage);
            }
            else
            {
                if (response.Data != null)
                    return new IexStockQuote
                    {
                        Ticker = response.Data.symbol,
                        Open = response.Data.iexOpen ?? 0,
                        OpenTime = FromUnixTime(response.Data.iexOpenTime),
                        Close = response.Data.iexClose ?? 0,
                        CloseTime = FromUnixTime(response.Data.iexCloseTime),
                        LatestPrice = response.Data.latestPrice ?? 0,
                        LatestTime = DateTime.Parse(response.Data.latestTime),
                        LatestUpdateTime = FromUnixTime(response.Data.latestUpdate),
                        LatestVolume = response.Data.latestVolume ?? 0,
                        DelayedPrice = response.Data.delayedPrice ?? 0,
                        DelayedPriceTime = FromUnixTime(response.Data.delayedPriceTime),
                        PreviousClose = response.Data.previousClose ?? 0,
                        IexRealTimePrice = response.Data.iexRealtimePrice ?? 0,
                        IexRealTimeSize = response.Data.iexRealtimeSize ?? 0,
                        IexLastUpdated = FromUnixTime(response.Data.lastTradeTime),
                        IexBidPrice = response.Data.iexBidPrice ?? 0,
                        IexBidSize = response.Data.iexBidSize ?? 0,
                        IexAskPrice = response.Data.iexAskPrice ?? 0,
                        IexAskSize = response.Data.iexAskSize ?? 0,
                        Change = decimal.ToDouble(response.Data.change ?? 0),
                        ChangePercent = decimal.ToDouble(response.Data.changePercent ?? 0),
                        MarketCap = response.Data.marketCap ?? 0,
                        PeRatio = decimal.ToDouble(response.Data.peRatio ?? 0),
                        Week52High = response.Data.week52High ?? 0,
                        Week52Low = response.Data.week52Low ?? 0,
                        YtdChange = decimal.ToDouble(response.Data.ytdChange ?? 0)
                    };
            }
        }

        return new IexStockQuote();
    }

    /// <summary>
    ///     Converts Unix milliseconds to DateTime.
    /// </summary>
    /// <param name="uTime">Unix Time.</param>
    /// <returns>A DateTime.</returns>
    private static DateTime FromUnixTime(decimal? uTime)
    {
        if (!uTime.HasValue) throw new ArgumentNullException(nameof(uTime));

        long milliseconds = long.Parse(uTime.ToString());
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds)
            .DateTime.ToLocalTime();
    }
}