// <copyright file="AlphaVantageMarketDataValuesController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Services;

[Route("api")]
public class AlphaVantageMarketDataValuesController : ControllerBase
{
    /// <summary>
    ///     The AlphaVantageService instance.
    /// </summary>
    private readonly IAlphaVantageService _avService;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<AlphaVantageMarketDataValuesController> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaVantageMarketDataValuesController" /> class.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    /// <param name="avService">The AlphaVantage API Service.</param>
    public AlphaVantageMarketDataValuesController(
        ILogger<AlphaVantageMarketDataValuesController> logger,
        IAlphaVantageService avService)
    {
        _logger = logger;
        _avService = avService;
    }

    /// <summary>
    ///     Gets the AlphaVantage Stock data for a given ticker within the date range and period.
    /// </summary>
    /// <param name="ticker">The Stock Ticker.</param>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <param name="period">The data period range.</param>
    /// <returns>An IActionResult.</returns>
    [Route("AVEod/{ticker}/{start}/{end}/{period}")]
    [HttpGet]
    public async Task<IActionResult> GetAvStockEod(
        string ticker,
        string start,
        string end,
        string period
    )
    {
        if (string.IsNullOrWhiteSpace(ticker)) return BadRequest("Ticker is invalid");
        if (string.IsNullOrWhiteSpace(start) || !IsDateValid(start))
            return BadRequest("Start Date is invalid");
        if (string.IsNullOrWhiteSpace(end) || !IsDateValid(end)) return BadRequest("End Date is invalid");
        if (string.IsNullOrWhiteSpace(period)) return BadRequest("Period is invalid");

        var data = await _avService.GetStockEOD(ticker, start, end, period);

        if (!data.Any()) return NotFound($"No data for Ticker: {ticker}");

        return Ok(data);
    }

    [Route("AVBar/{ticker}/{interval:int}/{outputSize:int}")]
    [HttpGet]
    public async Task<IActionResult> GetAvStockBar(string ticker, int interval, int outputSize)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return BadRequest("Ticker is invalid");
        if (interval < 1) return BadRequest("interval is invalid. Must be greater than 0.");
        if (outputSize < 1) return BadRequest("outputSize is invalid. Must be greater than 0.");

        var data = await _avService.GetStockBar(ticker, interval, outputSize);

        if (!data.Any()) return NotFound($"No data for Ticker: {ticker}");

        return Ok(data);
    }

    [Route("AVQuote/{ticker}")]
    [HttpGet]
    public async Task<IActionResult> GetAvQuote(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return BadRequest("Ticker is invalid");

        var data = await _avService.GetStockQuote(ticker);

        if (string.IsNullOrWhiteSpace(data.Ticker)) return NotFound($"No data for Ticker: {ticker}");

        return Ok(data);
    }

    /// <summary>
    ///     Checks if a string is a valid yyy-MM-dd date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if valid else false.</returns>
    private static bool IsDateValid(string date)
    {
        return DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None,
            out _);
    }
}