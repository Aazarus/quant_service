// <copyright file="StockOutput.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

public class StockOutput
{
    /// <summary>
    ///     Gets or sets the status of the connection.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Gets or sets the StockData.
    /// </summary>
    public StockData? Stock { get; set; }

    /// <summary>
    ///     Gets or sets the Total Data Points from the endpoint.
    /// </summary>
    public int TotalDataPoints { get; set; }
}