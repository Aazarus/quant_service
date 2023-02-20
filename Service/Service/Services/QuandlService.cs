// <copyright file="QuandlService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;
using Models.Quandl;
using Quandl.NET.Model.Response;
using Wrapper;

public class QuandlService : IQuandlService
{
    /// <summary>
    ///     The Quandl Api Wrapper.
    /// </summary>
    private readonly IQuandlWrapper _apiWrapper;

    /// <summary>
    ///     The Application Logger.
    /// </summary>
    private readonly ILogger<QuandlService> _logger;

    /// <summary>
    ///     The API Keys.
    /// </summary>
    private readonly ApiKeySettings.Quandl _quandlKey;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QuandlService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="quandlKey">The Quandl Api Keys.</param>
    /// <param name="apiWrapper">The Quandl Api Wrapper.</param>
    public QuandlService(ILogger<QuandlService> logger, ApiKeySettings.Quandl quandlKey, IQuandlWrapper apiWrapper)
    {
        _logger = logger;
        _quandlKey = quandlKey;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<List<QuandlStockData>> GetQuandlStock(string ticker, string start, string end)
    {
        var result = await _apiWrapper.GetQuandlStock(_quandlKey.ApiKey, ticker, start, end);
        return ProcessQuandlResultForQuandlStockData(result, ticker);
    }

    private List<QuandlStockData> ProcessQuandlResultForQuandlStockData(TimeseriesDataResponse result, string ticker)
    {
        var models = new List<QuandlStockData>();

        foreach (object[]? data in result.DatasetData.Data)
            try
            {
                models.Add(new QuandlStockData
                {
                    Ticker = ticker,
                    Date = DateTime.Parse(data[0].ToString()),
                    Open = decimal.Parse(data[1].ToString()),
                    High = decimal.Parse(data[2].ToString()),
                    Low = decimal.Parse(data[3].ToString()),
                    Close = decimal.Parse(data[4].ToString()),
                    Volume = decimal.Parse(data[5].ToString()),
                    ExDividend = decimal.Parse(data[6].ToString()),
                    SplitRatio = decimal.Parse(data[7].ToString()),
                    OpenAdj = decimal.Parse(data[8].ToString()),
                    HighAdj = decimal.Parse(data[9].ToString()),
                    LowAdj = decimal.Parse(data[10].ToString()),
                    VolumeAdj = decimal.Parse(data[11].ToString())
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process QuandlStockData result item: {ex.Message}");
            }

        return models.OrderBy(d => d.Date).ToList();
    }
}