// <copyright file="IexStockData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models.IEX;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class IexStockData
{
    /// <summary>
    ///     Gets or sets the Ticker.
    /// </summary>
    public string Ticker { get; set; }

    /// <summary>
    ///     Gets or sets the Open price
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    ///     Gets or sets the Time for the Open price
    /// </summary>
    public DateTime OpenTime { get; set; }

    /// <summary>
    ///     Gets or sets the Close Price
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    ///     Gets or sets the the Time for the Close Price
    /// </summary>
    public DateTime CloseTime { get; set; }

    /// <summary>
    ///     Gets or sets the Latest Price
    /// </summary>
    public decimal LatestPrice { get; set; }

    /// <summary>
    ///     Get or sets the Time for the Latest Price
    /// </summary>
    public DateTime LatestTime { get; set; }

    /// <summary>
    ///     Get or sets the Time for the Latest Price
    /// </summary>
    public DateTime LatestUpdateTime { get; set; }
}