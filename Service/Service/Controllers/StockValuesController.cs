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
    /// <summary>
    ///     The DB context.
    /// </summary>
    private readonly QuantDataContext _context;

    /// <summary>
    ///     The Application Logger
    /// </summary>
    private readonly ILogger<StockValuesController> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StockValuesController" /> class.
    /// </summary>
    /// <param name="logger">The Application logger.</param>
    /// <param name="context">The DB context.</param>
    public StockValuesController(
        ILogger<StockValuesController> logger,
        QuantDataContext context)
    {
        _context = context;
        _logger = logger;
        ConfirmReady();
    }

    /// <summary>
    ///     Gets all the Symbols.
    /// </summary>
    /// <returns>A collection of Symbols.</returns>
    [HttpGet]
    public IEnumerable<Symbol>? GetSymbols()
    {
        return _context.Symbols;
    }

    /// <summary>
    ///     Gets a single Symbol by Id.
    /// </summary>
    /// <param name="id">The Id of the Symbol.</param>
    /// <returns>An ActionResult.</returns>
    [HttpGet("{id:int}")]
    public Symbol? GetSymbol(int id)
    {
        return _context.Symbols!.FirstOrDefault(symbol => symbol.SymbolId == id);
    }

    [HttpGet("{id:int}/{start}/{end}")]
    public IActionResult GetSymbolAndPrices(int id, string start, string end)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            return BadRequest("Invalid argument provided");

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

    /// <summary>
    ///     Gets a single Symbol by the Ticker.
    /// </summary>
    /// <param name="ticker">The Ticker for the Symbol.</param>
    /// <returns>A Symbol.</returns>
    [HttpGet("with-ticker/{ticker}")]
    public Symbol? GetSymbolWithTicker(string ticker)
    {
        return _context.Symbols!.FirstOrDefault(t => t.Ticker == ticker);
    }

    /// <summary>
    ///     Gets a single Symbol with Prices.
    /// </summary>
    /// <param name="ticker">The Ticker for the Symbol.</param>
    /// <param name="start">The Start date of the search.</param>
    /// <param name="end">The End date of the search.</param>
    /// <returns>An ActionResult</returns>
    [HttpGet("and-prices-with-ticker/{ticker}/{start}/{end}")]
    public IActionResult GetSymbolAndPriceWithTicker(string ticker, string start, string end)
    {
        if (string.IsNullOrEmpty(ticker) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            return BadRequest("Invalid argument provided");

        var startDate = DateTime.ParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var stock = _context.Symbols!.FirstOrDefault(sym => sym.Ticker == ticker);

        if (stock == null) return NotFound($"Symbol with ticker: '{ticker}' not found.");

        stock.Prices = _context.Prices!.Where(p =>
                p.SymbolId == stock.SymbolId &&
                p.Date >= startDate &&
                p.Date <= endDate)
            .OrderBy(d => d.Date).ToList();

        if (!stock.Prices.Any())
            return NotFound(
                $"Symbol with ticker: '{ticker}' found but no Prices available for given start and end dates.");

        return Ok(stock);
    }

    /// <summary>
    ///     Creates a new entry for a valid Symbol.
    /// </summary>
    /// <param name="symbol">The Symbol to add.</param>
    /// <returns>An ActionResult.</returns>
    [HttpPost]
    public IActionResult CreateSymbol([FromBody] Symbol symbol)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingSymbol = GetSymbolWithTicker(symbol.Ticker!);
        if (existingSymbol != null) return Ok(existingSymbol.SymbolId);

        _context.Add(symbol);
        _context.SaveChanges();
        return Ok(symbol.SymbolId);
    }

    /// <summary>
    ///     Updates an existing Symbol.
    /// </summary>
    /// <param name="id">The ID of the symbol to update.</param>
    /// <param name="symbol">The updated Symbol.</param>
    /// <returns>An ActionResult.</returns>
    [HttpPut("{id:int}")]
    public IActionResult UpdateSymbol(int id, [FromBody] Symbol symbol)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_context.Symbols!.Any(sym => sym.SymbolId == id))
            return BadRequest($"Symbol with Id: '{symbol.SymbolId}' does not exist.");

        _context.Symbols!.Update(symbol);
        _context.SaveChanges();
        return Ok(symbol.SymbolId);
    }

    /// <summary>
    ///     Deletes a Symbol from the Database along with any Prices.
    /// </summary>
    /// <param name="id">The Id of the Symbol.</param>
    [HttpDelete("{id:int}")]
    public void DeleteStock(int id)
    {
        var prices = _context.Prices!.FirstOrDefault(p => p.SymbolId == id);
        if (prices != null) _context.Prices!.RemoveRange(prices);

        _context.Symbols!.Remove(new Symbol {SymbolId = id});
        _context.SaveChanges();
    }

    /// <summary>
    ///     Gets all the IndexData.
    /// </summary>
    /// <returns>A collection of IndexData.</returns>
    [HttpGet("index-data")]
    public IEnumerable<IndexData>? GetIndexData()
    {
        return _context.IndexData;
    }

    /// <summary>
    ///     Gets all the IndexData within the Start and End date.
    /// </summary>
    /// <param name="start">The Start date of the search.</param>
    /// <param name="end">The End date of the search.</param>
    /// <returns>An ActionResult.</returns>
    [HttpGet("index-data/{start}/{end}")]
    public IActionResult GetIndexData(string start, string end)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            return BadRequest("Invalid argument provided");

        var startDate = DateTime.ParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var indexData = _context.IndexData!
            .Where(d => d.Date >= startDate && d.Date <= endDate)
            .OrderBy(d => d.Date)
            .ToList();

        return Ok(indexData);
    }

    /// <summary>
    ///     Adds the historical end of day data for a collection of stocks
    /// </summary>
    /// <param name="stocks">The stocks to get data for.</param>
    /// <returns>An IActionResult.</returns>
    [HttpPost("add-stock-price")]
    public IActionResult AddStockPrice([FromBody] List<StockData> stocks)
    {
        if (!stocks.Any()) return BadRequest("No stocks provided");

        var symbol = GetSymbolWithTicker(stocks.First().Ticker!);
        if (symbol == null) return BadRequest($"Ticker '{stocks.First().Ticker!}' is unknown.");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var prices = (from stock in stocks
            where !_context.Prices!.Any(p => p.Date == stock.Date && p.SymbolId == symbol.SymbolId)
            select new Price
            {
                SymbolId = symbol.SymbolId,
                Date = stock.Date,
                Open = stock.Open,
                High = stock.High,
                Low = stock.Low,
                Close = stock.Close,
                CloseAdj = stock.CloseAdj,
                Volume = stock.Volume
            }).ToList();

        _context.Prices!.AddRange(prices);
        _context.SaveChanges();

        return Ok(prices);
    }

    private void ConfirmReady()
    {
        // ToDo: If we plugged in a 3rd party log monitor (e.g. NewRelic) then we could use that to monitor issues.
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

        if (_context.IndexData == null)
        {
            _logger.LogCritical("No IndexData available.");
            throw new InvalidOperationException("No IndexData available.");
        }

        _logger.LogInformation("Data checked and ready.");
    }
}