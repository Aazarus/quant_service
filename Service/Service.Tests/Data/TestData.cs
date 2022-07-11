// <copyright file="TestData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Data;

using System;
using System.Collections.Generic;
using IEXSharp.Model;
using IEXSharp.Model.CoreData.StockPrices.Response;
using IEXSharp.Model.Shared.Response;
using Models;
using Models.IEX;
using NodaTime;
using YahooQuotesApi;
using Symbol = Models.Symbol;

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
            Volume = 4634400m,
            SymbolId = 1
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
            Volume = 4776500m,
            SymbolId = 2
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
            Volume = 4307300m,
            SymbolId = 2
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
            Volume = 5107500m,
            SymbolId = 1
        }
    };

    public static IEnumerable<PriceTick> SecurityPriceHistory = new List<PriceTick>
    {
        new
        (
            LocalDate.FromDateTime(new DateTime(2018, 01, 01, 0, 0, 0)),
            123.321,
            124.12,
            122.891,
            123.019,
            123.0,
            25565416
        ),
        new
        (
            LocalDate.FromDateTime(new DateTime(2018, 01, 02, 0, 0, 0)),
            124.321,
            125.12,
            122.491,
            123.019,
            123.50,
            25765416
        ),
        new
        (
            LocalDate.FromDateTime(new DateTime(2018, 01, 03, 0, 0, 0)),
            123.821,
            124.412,
            123.1991,
            123.719,
            123.70,
            37565416
        ),
        new
        (
            LocalDate.FromDateTime(new DateTime(2018, 01, 04, 0, 0, 0)),
            124.7521,
            125.412,
            123.1991,
            124.719,
            124.170,
            32565416
        )
    };

    public static IEnumerable<StockData> StockDataDaily = new List<StockData>
    {
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Now.AddDays(-365),
            Open = 123.321m,
            High = 124.321m,
            Low = 122.021m,
            Close = 124.320m,
            CloseAdj = 124.0m,
            Volume = 32425284
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Now.AddDays(-364),
            Open = 124.320m,
            High = 125.81m,
            Low = 122.021m,
            Close = 125.30m,
            CloseAdj = 125.0m,
            Volume = 32443284
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Now.AddDays(-363),
            Open = 125.30m,
            High = 125.481m,
            Low = 123.021m,
            Close = 124.30m,
            CloseAdj = 124.0m,
            Volume = 31425284
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Now.AddDays(-362),
            Open = 124.30m,
            High = 126.481m,
            Low = 122.021m,
            Close = 124.30m,
            CloseAdj = 123.5m,
            Volume = 41425284
        }
    };

    public static IEXResponse<IEnumerable<HistoricalPriceResponse>> IEXResponse = new()
    {
        Data = new List<HistoricalPriceResponse>
        {
            new()
            {
                date = DateTime.Now.AddDays(-365).ToShortDateString(),
                open = 123.321m,
                high = 124.321m,
                low = 122.021m,
                close = 124.320m,
                volume = 32425284
            },
            new()
            {
                date = DateTime.Now.AddDays(-364).ToShortDateString(),
                open = 124.320m,
                high = 125.81m,
                low = 122.021m,
                close = 125.30m,
                volume = 32443284
            },
            new()
            {
                date = DateTime.Now.AddDays(-363).ToShortDateString(),
                open = 125.30m,
                high = 125.481m,
                low = 123.021m,
                close = 124.30m,
                volume = 31425284
            },
            new()
            {
                date = DateTime.Now.AddDays(-362).ToShortDateString(),
                open = 124.30m,
                high = 126.481m,
                low = 122.021m,
                close = 124.30m,
                volume = 41425284
            }
        }
    };

    public static IEXResponse<Quote> IEXQuote = new()
    {
        // This is taken directly from the API request and may be updated in the future
        Data = new Quote
        {
            symbol = "IBM",
            companyName = "International Business Machines Corp.",
            primaryExchange = "NEW YORK STOCK EXCHANGE INC.",
            calculationPrice = "close",
            open = null,
            openTime = null,
            openSource = "official",
            close = null,
            closeTime = null,
            closeSource = "official",
            high = null,
            highTime = 1657569599993,
            highSource = "15 minute delayed price",
            low = null,
            lowTime = 1657549289791,
            lowSource = "IEX real time price",
            latestPrice = 141M,
            latestSource = "Close",
            latestTime = "July 11, 2022",
            latestUpdate = 1657569602157,
            latestVolume = null,
            iexRealtimePrice = 140.99M,
            iexRealtimeSize = 100,
            iexLastUpdated = 1657569596227,
            delayedPrice = null,
            delayedPriceTime = 1657569599993,
            oddLotDelayedPrice = null,
            oddLotDelayedPriceTime = 1657569599993,
            extendedPrice = null,
            extendedChange = null,
            extendedChangePercent = null,
            extendedPriceTime = null,
            previousClose = 140.47M,
            previousVolume = 2820928M,
            change = 0.53M,
            changePercent = 0.00377M,
            volume = null,
            iexMarketPercent = 0.04435251961671668M,
            iexVolume = 173127,
            avgTotalVolume = 5292188,
            iexBidPrice = 0,
            iexBidSize = 0,
            iexAskPrice = 0,
            iexAskSize = 0,
            iexOpen = 140.78M,
            iexOpenTime = 1657546201086M,
            iexClose = 140.99M,
            iexCloseTime = 1657569596227,
            marketCap = 126820380825,
            peRatio = 22.89M,
            week52High = 144.73M,
            week52Low = 111.84M,
            ytdChange = 0.0802814805155791M,
            lastTradeTime = 1657569599993,
            isUSMarketOpen = false,
            sector = null
        }
    };

    public static IexStockQuote StockQuote = new()
    {
        Ticker = IEXQuote.Data.symbol,
        Open = IEXQuote.Data.iexOpen ?? 0,
        OpenTime = FromUnixTime(IEXQuote.Data.iexOpenTime),
        Close = IEXQuote.Data.iexClose ?? 0,
        CloseTime = FromUnixTime(IEXQuote.Data.iexCloseTime),
        LatestPrice = IEXQuote.Data.latestPrice ?? 0,
        LatestTime = DateTime.Parse(IEXQuote.Data.latestTime),
        LatestUpdateTime = FromUnixTime(IEXQuote.Data.latestUpdate),
        LatestVolume = IEXQuote.Data.latestVolume ?? 0,
        DelayedPrice = IEXQuote.Data.delayedPrice ?? 0,
        DelayedPriceTime = FromUnixTime(IEXQuote.Data.delayedPriceTime),
        PreviousClose = IEXQuote.Data.previousClose ?? 0,
        IexRealTimePrice = IEXQuote.Data.iexRealtimePrice ?? 0,
        IexRealTimeSize = IEXQuote.Data.iexRealtimeSize ?? 0,
        IexLastUpdated = FromUnixTime(IEXQuote.Data.iexLastUpdated),
        IexBidPrice = IEXQuote.Data.iexBidPrice ?? 0,
        IexBidSize = IEXQuote.Data.iexBidSize ?? 0,
        IexAskPrice = IEXQuote.Data.iexAskPrice ?? 0,
        IexAskSize = IEXQuote.Data.iexAskSize ?? 0,
        Change = decimal.ToDouble(IEXQuote.Data.change ?? 0),
        ChangePercent = decimal.ToDouble(IEXQuote.Data.changePercent ?? 0),
        MarketCap = IEXQuote.Data.marketCap ?? 0,
        PeRatio = decimal.ToDouble(IEXQuote.Data.peRatio ?? 0),
        Week52High = IEXQuote.Data.week52High ?? 0,
        Week52Low = IEXQuote.Data.week52Low ?? 0,
        YtdChange = decimal.ToDouble(IEXQuote.Data.ytdChange ?? 0)
    };

    // This is a copy of IEXService method. Possibly worth reusing that version.
    private static DateTime FromUnixTime(decimal? uTime)
    {
        if (!uTime.HasValue) throw new ArgumentNullException(nameof(uTime));

        long milliseconds = long.Parse(uTime.ToString());
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds)
            .DateTime.ToLocalTime();
    }
}