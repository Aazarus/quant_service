// <copyright file="Price.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

public class Price
{
    public int PriceId { get; set; }
    public int SymbolId { get; set; }
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal CloseAdj { get; set; }
    public decimal Volume { get; set; }
}