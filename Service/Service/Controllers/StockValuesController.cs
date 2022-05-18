// <copyright file="StockValuesController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Models;

[Route("api/Stocks")]
public class StockValuesController : ControllerBase
{
    private readonly QuantDataContext _context;
    private readonly ILogger<StockValuesController> _logger;

    public StockValuesController(
        QuantDataContext context,
        ILogger<StockValuesController> logger)
    {
        _context = context;
        _logger = logger;
        ConfirmReady();
    }

    [HttpGet]
    public IEnumerable<Symbol>? GetSymbols()
    {
        return _context.Symbols;
    }

    [HttpGet("{id:int}")]
    public Symbol? GetSymbol(int id)
    {
        return _context.Symbols!.FirstOrDefault(symbol => symbol.SymbolId == id);
    }

    [HttpGet("{id:int}/{start}/{end}")]
    public IActionResult GetSymbolAndPrices(int id, string start, string end)
    {
        var startDate = DateTime.ParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var stock = _context.Symbols!.FirstOrDefault(sym => sym.SymbolId == id);

        if (stock == null) return NotFound($"Symbol with id: '{id}' not found.");

        stock.Prices = _context.Prices!.Where(p =>
                p.SymbolId == id &&
                p.Date >= startDate &&
                p.Date <= endDate)
            .OrderBy(d => d.Date).ToList();

        if (!stock.Prices.Any())
            return NotFound($"Symbol with id: '{id}' found but no Prices available for given start and end dates.");

        return Ok(stock);
    }

    private void ConfirmReady()
    {
        if (_context.Symbols == null)
        {
            _logger.LogCritical("No Symbols available.");
            throw new InvalidOperationException("No Symbols available.");
        }

        if (_context.Prices == null)
        {
            _logger.LogCritical("No Prices available.");
            throw new InvalidOperationException("No Prices available.");
        }

        _logger.LogInformation("Data checked and ready.");
    }
}