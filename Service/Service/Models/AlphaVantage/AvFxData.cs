// <copyright file="AvFxData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models.AlphaVantage;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AvFxData
{
    public string? Ticker { get; set; }
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}