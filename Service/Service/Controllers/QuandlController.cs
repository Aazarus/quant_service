// <copyright file="QuandlController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;

[Route("api")]
public class QuandlController : Controller
{
    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<QuandlController> _logger;

    /// <summary>
    ///     The QuandlService instance.
    /// </summary>
    private readonly IQuandlService _quandlService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QuandlController" /> class.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    /// <param name="quandlService">The Quandl API Service.</param>
    public QuandlController(
        ILogger<QuandlController> logger,
        IQuandlService quandlService)
    {
        _logger = logger;
        _quandlService = quandlService;
    }

    [Route("QuandlStock/{ticker}/{start}/{end}")]
    [HttpGet]
    public async Task<IActionResult> GetQuandlStock(string ticker, string start, string end)
    {
        if (string.IsNullOrWhiteSpace(ticker)) return BadRequest("Ticker is invalid");
        if (string.IsNullOrWhiteSpace(start)) return BadRequest("Start Date is invalid");
        if (string.IsNullOrWhiteSpace(end)) return BadRequest("Period is invalid");

        var data = await _quandlService.GetQuandlStock(ticker, start, end);

        if (!data.Any()) return NotFound($"No data for Quandl Ticker: '{ticker}' between '{start}' and '{end}' dates.");

        return Ok(data);
    }
}