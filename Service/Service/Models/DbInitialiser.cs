// <copyright file="DbInitialiser.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Helpers;

[ExcludeFromCodeCoverage]
public class DbInitialiser
{
    public static void Initialise(QuantDataContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context), "Context cannot be null.");

        context.Database.EnsureCreated();

        if (context.Symbols != null &&
            context.IndexData != null &&
            context.Prices != null &&
            context.IndexData.Any() &&
            context.Symbols.Any() &&
            context.Prices.Any()
           )
            return;

        string path = AppContext.BaseDirectory;
        string[] ss = Regex.Split(path, "bin");
        string filePath = ss[0] + @"DbData\";

        if (!Directory.Exists(filePath)) throw new DirectoryNotFoundException("Model Helper directory not found.");

        if (context.Symbols != null && !context.Symbols.Any())
        {
            var symbols = ModelHelper.CsvToSymbolList(filePath + "StockTickers.csv");
            foreach (var symbol in symbols) context.Symbols?.Add(symbol);

            // Prices relies on Symbol so need to ensure this is processed first.
            context.SaveChanges();
        }

        if (context.IndexData != null && !context.IndexData.Any())
        {
            var data = ModelHelper.CsvToIndexData(filePath + "Indices.csv");

            foreach (var d in data) context.IndexData?.Add(d);
        }

        if (context.Prices != null && !context.Prices.Any())
        {
            var prices = ModelHelper.CsvToIbmPrices("IBM", context, filePath + "IBM.csv");
            foreach (var price in prices) context.Prices?.Add(price);
        }

        context.SaveChanges();
    }
}