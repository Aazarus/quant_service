// <copyright file="StockData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class StockData
{
    public string? Ticker { get; set; }
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal CloseAdj { get; set; }
    public decimal Volume { get; set; }
}