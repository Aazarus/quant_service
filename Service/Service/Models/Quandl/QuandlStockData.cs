// <copyright file="QuandlStockData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models.Quandl;

public class QuandlStockData
{
    /// <summary>
    ///     Gets or sets the Ticker.
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    ///     Gets or sets the Stock Data Date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Gets or sets the Open price.
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    ///     Gets or sets the High price.
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    ///     Gets or sets the Low price.
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    ///     Gets or sets the Close price.
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    ///     Gets or sets the Volume.
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    ///     Gets or sets the Open adjusted price.
    /// </summary>
    public decimal OpenAdj { get; set; }

    /// <summary>
    ///     Gets or sets the High adjusted price.
    /// </summary>
    public decimal HighAdj { get; set; }

    /// <summary>
    ///     Gets or sets the Low adjusted price.
    /// </summary>
    public decimal LowAdj { get; set; }

    /// <summary>
    ///     Gets or sets the Close adjusted price.
    /// </summary>
    public decimal CloseAdj { get; set; }

    /// <summary>
    ///     Gets or sets the Volume adjusted price.
    /// </summary>
    public decimal VolumeAdj { get; set; }

    /// <summary>
    ///     Gets or sets the ExDividend price.
    /// </summary>
    public decimal ExDividend { get; set; }

    /// <summary>
    ///     Gets or sets the split ratio.
    /// </summary>
    public decimal SplitRatio { get; set; }
}