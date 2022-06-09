// <copyright file="YahooMarketDataValuesController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

[Produces("application/json")]
[Route("~/api/YahooStock")]
public class YahooMarketDataValuesController
{
    private readonly ILogger<YahooMarketDataValuesController> _logger;
    private readonly IYahooService _yahooService;

    public YahooMarketDataValuesController(
        ILogger<YahooMarketDataValuesController> logger,
        IYahooService yahooService)
    {
        _logger = logger;
        _yahooService = yahooService;
    }

    /// <summary>
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    [Route("{ticker}/{start}/{end}/{period}")]
    [HttpGet]
    public async Task<IEnumerable<StockData>> GetYahooStock(string ticker, string start, string end, string period)
    {
        if (string.IsNullOrEmpty(ticker)) throw new ArgumentNullException($"Ticker '{ticker}' is invalid.");

        if (string.IsNullOrEmpty(start)) throw new ArgumentNullException($"Start Date '{start}' in invalid.");

        if (string.IsNullOrEmpty(end)) throw new ArgumentNullException($"End Date '{end}' in invalid.");

        if (string.IsNullOrEmpty(period)) throw new ArgumentNullException($"Period '{period}' in invalid.");

        return await _yahooService.GetStockDataWithPrices(ticker, DateTime.Parse(start), DateTime.Parse(end), period);
    }
}