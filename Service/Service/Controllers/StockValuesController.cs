// <copyright file="StockValuesController.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Controllers;

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
    }

    [HttpGet]
    public IEnumerable<Symbol> GetSymbols()
    {
        return _context.Symbols!;
    }

    [HttpGet(":id")]
    public Symbol GetSymbol(int id)
    {
        return (_context.Symbols == null
                   ? new Symbol()
                   : _context.Symbols.FirstOrDefault(symbol => symbol.SymbolId == id)) ??
               throw new InvalidOperationException($"Could not find Symbol with id: {id}");
    }
}