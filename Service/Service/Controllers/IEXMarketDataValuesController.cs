// <copyright file="IEXMarketDataValuesController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using IEXSharp.Model.CoreData.StockPrices.Request;
using Microsoft.AspNetCore.Mvc;
using Services;

[Route("api/IexStock")]
public class IEXMarketDataValuesController : ControllerBase
{
    /// <summary>
    ///     The IEXService instance.
    /// </summary>
    private readonly IIEXService _iexService;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<IEXMarketDataValuesController> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IEXMarketDataValuesController" /> class.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    /// <param name="iexService">The IEX API Service</param>
    public IEXMarketDataValuesController(
        ILogger<IEXMarketDataValuesController> logger,
        IIEXService iexService)
    {
        _logger = logger;
        _iexService = iexService;
    }

    /// <summary>
    ///     Gets the IEX Stock data for a given ticker within the provided range.
    /// </summary>
    /// <param name="ticker">The Stock Ticker.</param>
    /// <param name="range">The data range.</param>
    /// <returns>An IActionResult.</returns>
    [Route("{ticker}/{range}")]
    [HttpGet]
    public async Task<IActionResult> GetIexStock(
        string ticker,
        ChartRange range =
            ChartRange.OneMonth
    )
    {
        if (string.IsNullOrWhiteSpace(ticker)) return BadRequest("Ticker is invalid");

        var data = await _iexService.GetStock(ticker, range);

        if (!data.Any()) return NotFound($"No data for Ticker: {ticker}");

        return Ok(data);
    }
}