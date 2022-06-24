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
    /// <summary>
    ///     The Application logger.
    /// </summary>
    private readonly ILogger<YahooMarketDataValuesController> _logger;

    /// <summary>
    ///     The Yahoo Service.
    /// </summary>
    private readonly IYahooService _yahooService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="YahooMarketDataValuesController" /> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="yahooService"></param>
    public YahooMarketDataValuesController(
        ILogger<YahooMarketDataValuesController> logger,
        IYahooService yahooService)
    {
        _logger = logger;
        _yahooService = yahooService;
    }

    /// <summary>
    ///     Gets the StockData for a given ticker between start and end dates with period between items.
    /// </summary>
    /// <param name="ticker">The ticker.</param>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <param name="period">The time period between items.</param>
    /// <returns>A collection of StockData.</returns>
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