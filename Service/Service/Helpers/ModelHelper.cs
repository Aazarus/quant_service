// <copyright file="ModelHelper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Helpers;

using Models;

public class ModelHelper
{
    public static List<Symbol> CsvToSymbolList(string csvFile)
    {
        //string csvFile = @"..\Models\StockTickers.csv";
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        string[] fields = lst[0].Split(new[] {','});
        var res = new List<Symbol>();
        for (var i = 1; i < lst.Count; i++)
        {
            fields = lst[i].Split(',');
            res.Add(new Symbol
            {
                Ticker = fields[0],
                Region = fields[1],
                Sector = fields[2]
            });
        }

        return res;
    }

    public static List<IndexData> CsvToIndexData(string csvFile)
    {
        //string csvFile = @"..\Models\Data\indices.csv";
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        string[] fields = lst[0].Split(new[] {','});
        var res = new List<IndexData>();
        for (var i = 1; i < lst.Count; i++)
        {
            fields = lst[i].Split(',');
            res.Add(new IndexData
            {
                Date = DateTime.Parse(fields[0]),
                IGSpread = decimal.Parse(fields[1]),
                HYSpread = decimal.Parse(fields[2]),
                SPX = decimal.Parse(fields[3]),
                VIX = decimal.Parse(fields[4])
            });
        }

        return res;
    }

    public static List<Price> CsvToIbmPrices(string ticker, QuantDataContext context, string csvFile)
    {
        int symbolId = TickerToId(ticker, context);
        var fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        var lst = new List<string>();
        while (!sr.EndOfStream)
            lst.Add(sr.ReadLine());

        string[] fields = lst[0].Split(new[] {','});
        var res = new List<Price>();
        for (var i = 1; i < lst.Count; i++)
        {
            fields = lst[i].Split(',');
            res.Add(new Price
            {
                SymbolId = symbolId,
                Date = DateTime.Parse(fields[0]),
                Open = decimal.Parse(fields[1]),
                High = decimal.Parse(fields[2]),
                Low = decimal.Parse(fields[3]),
                Close = decimal.Parse(fields[4]),
                CloseAdj = decimal.Parse(fields[5]),
                Volume = decimal.Parse(fields[6])
            });
        }

        return res;
    }

    public static int TickerToId(string ticker, QuantDataContext context)
    {
        var id = 0;
        var symbol = new Symbol();
        var query = from s in context.Symbols
            where s.Ticker == ticker
            select s.SymbolId;
        foreach (int q in query)
            id = q;
        return id;
    }
}