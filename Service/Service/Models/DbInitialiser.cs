// <copyright file="DbInitialiser.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Text.RegularExpressions;
using Helpers;

public class DbInitialiser
{
    public static void Initialise(QuantDataContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context), "Context cannot be null.");

        context.Database.EnsureCreated();

        if (context.Symbols != null && context.IndexData != null && context.IndexData.Any() &&
            context.Symbols.Any())
            return;

        string path = AppContext.BaseDirectory;
        string[] ss = Regex.Split(path, "bin");
        string filePath = ss[0] + @"DbData\";

        if (!Directory.Exists(filePath)) throw new DirectoryNotFoundException("Model Helper directory not found.");

        var data = ModelHelper.CsvToIndexData(filePath + "Indices.csv");

        foreach (var d in data) context.IndexData?.Add(d);

        context.SaveChanges();

        var symbols = ModelHelper.CsvToSymbolList(filePath + "StockTickers.csv");
        foreach (var symbol in symbols) context.Symbols?.Add(symbol);

        context.SaveChanges();

        var prices = ModelHelper.CsvToIbmPrices("IBM", context, filePath + "IBM.csv");
        foreach (var price in prices) context.Prices?.Add(price);

        context.SaveChanges();
    }
}