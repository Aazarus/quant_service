// <copyright file="TestData.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Data;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public static IEXResponse<Quote> IEXQuoteException = new()
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
            iexLastUpdated = null,
            delayedPrice = null,
            delayedPriceTime = null,
            oddLotDelayedPrice = null,
            oddLotDelayedPriceTime = null,
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
        IexLastUpdated = FromUnixTime(IEXQuote.Data.lastTradeTime),
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

    public static string AvEODResponse = @"timestamp,open,high,low,close,adjusted close,volume,dividend amount
2022-08-11,133.1000,133.3500,129.1200,132.5400,132.5400,14467215,1.6500
2022-08-05,130.7500,132.8620,130.5100,132.4800,130.8129,17400572,0.0000
2022-07-29,128.4400,131.0000,127.5800,130.7900,129.1442,22223785,0.0000
2022-07-22,140.1500,140.3100,125.1300,128.2500,126.6361,66246811,0.0000
2022-07-15,140.6200,141.8700,135.0200,139.9200,138.1593,21089228,0.0000
2022-07-08,139.9700,141.3250,135.2700,140.4700,138.7023,16229131,0.0000";

    public static IEnumerable<StockData> AvEODData = new List<StockData>
    {
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-11"),
            Open = 133.1M,
            High = 133.35M,
            Low = 129.12M,
            Close = 132.54M,
            CloseAdj = 132.54M,
            Volume = 14467215M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-05"),
            Open = 130.75M,
            High = 132.862M,
            Low = 130.51M,
            Close = 132.48M,
            CloseAdj = 130.8129M,
            Volume = 17400572
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-07-29"),
            Open = 128.44M,
            High = 131.00M,
            Low = 127.58M,
            Close = 130.79M,
            CloseAdj = 129.1442M,
            Volume = 22223785
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-07-22"),
            Open = 140.15M,
            High = 140.31M,
            Low = 125.13M,
            Close = 128.25M,
            CloseAdj = 126.6361M,
            Volume = 66246811
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-07-15"),
            Open = 140.62M,
            High = 141.87M,
            Low = 135.02M,
            Close = 139.92M,
            CloseAdj = 138.1593M,
            Volume = 21089228
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-07-08"),
            Open = 139.97M,
            High = 141.325M,
            Low = 135.27M,
            Close = 140.47M,
            CloseAdj = 138.7023M,
            Volume = 16229131
        }
    };

    public static string AvBarResponse = @"timestamp,open,high,low,close,volume
2022-08-12 18:00:00,133.8800,134.0000,133.8800,134.0000,1303
2022-08-12 17:00:00,134.0100,134.0998,133.9700,134.0000,67834
2022-08-12 16:00:00,133.6100,134.0900,133.5800,134.0000,581400
2022-08-12 15:00:00,133.6200,133.7800,133.5650,133.6200,255033
2022-08-12 14:00:00,133.4300,133.6500,133.4200,133.6100,194805
2022-08-12 13:00:00,133.2600,133.5200,133.2450,133.4300,244128";

    public static IEnumerable<StockData> AvBarData = new List<StockData>
    {
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 18:00:00"),
            Open = 133.88M,
            High = 134.00M,
            Low = 133.88M,
            Close = 134.00M,
            Volume = 1303M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 17:00:00"),
            Open = 134.01M,
            High = 134.0998M,
            Low = 133.97M,
            Close = 134.00M,
            Volume = 67834M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 16:00:00"),
            Open = 133.61M,
            High = 134.09M,
            Low = 133.58M,
            Close = 134.00M,
            Volume = 581400M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 15:00:00"),
            Open = 133.6200M,
            High = 133.78M,
            Low = 133.565M,
            Close = 133.62M,
            Volume = 255033M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 14:00:00"),
            Open = 133.43M,
            High = 133.65M,
            Low = 133.42M,
            Close = 133.61M,
            Volume = 194805M
        },
        new()
        {
            Ticker = "IBM",
            Date = DateTime.Parse("2022-08-12 13:00:00"),
            Open = 133.26M,
            High = 133.52M,
            Low = 133.245M,
            Close = 133.43M,
            Volume = 244128M
        }
    };

    public static string AvQuoteResponse =
        @"symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent
IBM,132.6200,134.0900,131.9800,134.0100,2767054,2022-08-12,132.5400,1.4700,1.1091%";

    // This is a copy of IEXService method. Possibly worth reusing that version.
    [ExcludeFromCodeCoverage]
    private static DateTime FromUnixTime(decimal? uTime)
    {
        long milliseconds = long.Parse(uTime.ToString() ?? throw new ArgumentNullException(nameof(uTime)));
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds)
            .DateTime.ToLocalTime();
    }
}