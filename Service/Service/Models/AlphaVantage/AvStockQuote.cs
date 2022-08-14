// <copyright file="AvStockQuote.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models.AlphaVantage;

public class AvStockQuote
{
    public string? Ticker { get; set; }
    public DateTime TimeStamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    public decimal PrevClose { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
}