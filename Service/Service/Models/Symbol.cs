// <copyright file="Symbol.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

public class Symbol
{
    public int SymbolId { get; set; }
    public string Ticker { get; set; }
    public string Region { get; set; }
    public string Sector { get; set; }
    public virtual ICollection<Price> Prices { get; set; }
}