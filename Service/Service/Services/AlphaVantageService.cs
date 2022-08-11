﻿// <copyright file="AlphaVantageService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;
using Wrapper;

public class AlphaVantageService : IAlphaVantageService
{
    /// <summary>
    ///     The API Keys.
    /// </summary>
    private readonly ApiKeySettings.AlphaVantage _apiKey;

    /// <summary>
    ///     The IEX Api Wrapper.
    /// </summary>
    private readonly IAlphaVantageWrapper _apiWrapper;

    /// <summary>
    ///     The Application Logger.
    /// </summary>
    private readonly ILogger<AlphaVantageService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaVantageService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="apiKey">The AlphaVantage Api Keys.</param>
    /// <param name="apiWrapper">The AlphaVantage Wrapper.</param>
    public AlphaVantageService(ILogger<AlphaVantageService> logger, ApiKeySettings.AlphaVantage apiKey,
        IAlphaVantageWrapper apiWrapper)
    {
        _logger = logger;
        _apiKey = apiKey;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<List<StockData>> GetStockEOD(string ticker, string start, string end, string period)
    {
        string response = await _apiWrapper.GetStockEOD(ticker, start, period, _apiKey.ApiKey);
        return ProcessAVResponseForStockData(ticker, response, start, end).OrderBy(d => d.Date).ToList();
    }

    private IEnumerable<StockData> ProcessAVResponseForStockData(string ticker, string response, string start,
        string end)
    {
        var models = new List<StockData>();
        if (response != null)
        {
            if (response.Contains("This is a premium endpoint"))
                throw new NotSupportedException("Request requires Premium subscription to AlphaVantage.");

            if (response.Contains("parameter apikey is invalid or missing"))
            {
                _logger.LogError("Error calling AlphaVantage. API key may be invalid.");
                throw new Exception("Issue getting data from AlphaVantage");
            }

            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    var date = DateTime.Parse(r[0]);

                    if (date >= startDate && date <= endDate)
                        models.Add(new StockData
                        {
                            Ticker = ticker,
                            Date = date,
                            Open = decimal.Parse(r[1]),
                            High = decimal.Parse(r[2]),
                            Low = decimal.Parse(r[3]),
                            Close = decimal.Parse(r[4]),
                            CloseAdj = decimal.Parse(r[5]),
                            Volume = decimal.Parse(r[6])
                        });
                }
                catch (Exception ex)
                {
                    // ToDo: Log exception
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return models;
    }
}