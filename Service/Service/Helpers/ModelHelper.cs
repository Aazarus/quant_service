// <copyright file="ModelHelper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Helpers;

using System.Diagnostics.CodeAnalysis;
using Models;

[ExcludeFromCodeCoverage]
public class ModelHelper
{
    public static List<Symbol> CsvToSymbolList(string csvFile)
    {
        //string csvFile = @"..\Models\StockTickers.csv";
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string?>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        var res = new List<Symbol>();
        for (var i = 1; i < lst.Count; i++)
        {
            string[]? fields = lst[i]?.Split(',');
            res.Add(new Symbol
            {
                Ticker = fields?[0] ?? string.Empty,
                Region = fields?[1] ?? string.Empty,
                Sector = fields?[2] ?? string.Empty
            });
        }

        return res;
    }

    public static List<IndexData> CsvToIndexData(string csvFile)
    {
        //string csvFile = @"..\Models\Data\indices.csv";
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string?>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        var res = new List<IndexData>();
        for (var i = 1; i < lst.Count; i++)
        {
            string?[]? fields = lst[i]?.Split(',');
            res.Add(new IndexData
            {
                Date = DateTime.Parse(fields?[0] ?? string.Empty),
                IGSpread = decimal.Parse(fields?[1] ?? string.Empty),
                HYSpread = decimal.Parse(fields?[2] ?? string.Empty),
                SPX = decimal.Parse(fields?[3] ?? string.Empty),
                VIX = decimal.Parse(fields?[4] ?? string.Empty)
            });
        }

        return res;
    }

    public static List<Price> CsvToIbmPrices(string ticker, QuantDataContext context, string csvFile)
    {
        int symbolId = TickerToId(ticker, context);
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string?>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        var res = new List<Price>();
        for (var i = 1; i < lst.Count; i++)
        {
            string[]? fields = lst[i]?.Split(',');
            res.Add(new Price
            {
                SymbolId = symbolId,
                Date = DateTime.Parse(fields?[0] ?? string.Empty),
                Open = decimal.Parse(fields?[1] ?? string.Empty),
                High = decimal.Parse(fields?[2] ?? string.Empty),
                Low = decimal.Parse(fields?[3] ?? string.Empty),
                Close = decimal.Parse(fields?[4] ?? string.Empty),
                CloseAdj = decimal.Parse(fields?[5] ?? string.Empty),
                Volume = decimal.Parse(fields?[6] ?? string.Empty)
            });
        }

        return res;
    }

    public static int TickerToId(string ticker, QuantDataContext context)
    {
        var id = 0;
        var query = from s in context.Symbols
            where s.Ticker == ticker
            select s.SymbolId;
        foreach (int q in query)
            id = q;
        return id;
    }
}