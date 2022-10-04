// <copyright file="AvSectorPref.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class AvSectorPref
{
    public string Rank { get; set; }
    public string CommunicationServices { get; set; }
    public string ConsumerDiscretionary { get; set; }
    public string ConsumerStaples { get; set; }
    public string Energy { get; set; }
    public string Financials { get; set; }
    public string HealthCare { get; set; }
    public string Industrials { get; set; }
    public string InformationTechnology { get; set; }
    public string Materials { get; set; }
    public string Utilities { get; set; }
}