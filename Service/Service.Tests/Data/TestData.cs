// <copyright file="TestData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Data;

using System;
using System.Collections.Generic;
using Models;

public class TestData
{
    public static IEnumerable<Symbol> Symbols = new List<Symbol>
    {
        new() {Ticker = "IBM", Region = "US", Sector = "Information Technology", SymbolId = 1},
        new() {Ticker = "AAPL", Region = "US", Sector = "Information Technology", SymbolId = 2},
        new() {Ticker = "HMN", Region = "US", Sector = "Financials", SymbolId = 3},
        new() {Ticker = "AGO", Region = "US", Sector = "Financials", SymbolId = 4}
    };

    public static IEnumerable<IndexData> IndexData = new List<IndexData>
    {
        new()
        {
            Id = 1,
            Date = DateTime.Parse("10/03/2008"),
            IGSpread = 193.39m,
            HYSpread = 776.82m,
            SPX = 1273.37m,
            VIX = 29.38m
        },
        new()
        {
            Id = 2,
            Date = DateTime.Parse("11/03/2008"),
            IGSpread = 176.39m,
            HYSpread = 866.82m,
            SPX = 1353.37m,
            VIX = 27.38m
        },
        new()
        {
            Id = 3,
            Date = DateTime.Parse("11/03/2008"),
            IGSpread = 177.39m,
            HYSpread = 928.82m,
            SPX = 1753.37m,
            VIX = 24.38m
        },
        new()
        {
            Id = 4,
            Date = DateTime.Parse("12/03/2008"),
            IGSpread = 167.39m,
            HYSpread = 628.82m,
            SPX = 1853.37m,
            VIX = 31.38m
        }
    };

    public static IEnumerable<Price> Prices = new List<Price>
    {
        new()
        {
            PriceId = 1,
            Date = DateTime.Parse("07/11/2017"),
            Open = 151.37m,
            High = 151.51m,
            Low = 150.5m,
            Close = 151.35m,
            CloseAdj = 145.1933m,
            Volume = 3701100m,
            SymbolId = 1
        },
        new()
        {
            PriceId = 2,
            Date = DateTime.Parse("08/11/2017"),
            Open = 151.6m,
            High = 151.79m,
            Low = 150.28m,
            Close = 151.57m,
            CloseAdj = 145.4044m,
            Volume = 4634400m
        },
        new()
        {
            PriceId = 3,
            Date = DateTime.Parse("09/11/2017"),
            Open = 149.93m,
            High = 151.8m,
            Low = 149.86m,
            Close = 150.3m,
            CloseAdj = 145.6272m,
            Volume = 4776500m
        },
        new()
        {
            PriceId = 4,
            Date = DateTime.Parse("10/11/2017"),
            Open = 150.65m,
            High = 150.89m,
            Low = 149.14m,
            Close = 149.16m,
            CloseAdj = 144.5227m,
            Volume = 4307300m
        },
        new()
        {
            PriceId = 5,
            Date = DateTime.Parse("11/11/2017"),
            Open = 148.88m,
            High = 149.00m,
            Low = 147.92m,
            Close = 148.40m,
            CloseAdj = 143.7863m,
            Volume = 5107500m
        }
    };
}