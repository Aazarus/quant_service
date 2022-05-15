// <copyright file="IndexData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

public class IndexData
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal IGSpread { get; set; }
    public decimal HYSpread { get; set; }
    public decimal SPX { get; set; }
    public decimal VIX { get; set; }
}