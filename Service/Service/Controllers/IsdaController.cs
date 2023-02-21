// <copyright file="IsdaController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services;

[Route("api")]
public class IsdaController : Controller
{
    /// <summary>
    ///     The ISDA Service instance.
    /// </summary>
    private readonly IIsdaService _isdaService;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<IsdaController> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IsdaController" /> class.
    /// </summary>
    /// <param name="logger">The application logger.</param>
    /// <param name="isdaService">The ISDA API Service.</param>
    public IsdaController(
        ILogger<IsdaController> logger,
        IIsdaService isdaService)
    {
        _logger = logger;
        _isdaService = isdaService;
    }

    [Route("IsdaRate/{currency}/{date}")]
    [HttpGet]
    public async Task<IActionResult> GetIsdaRate(string currency, string date)
    {
        if (string.IsNullOrWhiteSpace(currency)) return BadRequest("Currency is invalid");
        if (string.IsNullOrWhiteSpace(date)) return BadRequest("Date is invalid");

        var data = await _isdaService.GetIsdaRates(currency, date);

        if (!data.Any()) return NotFound($"No data for currency '{currency}' for {date}");

        return Ok(data);
    }
}