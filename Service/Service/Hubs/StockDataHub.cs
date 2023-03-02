// <copyright file="StockDataHub.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Hubs;

using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Models;
using Services;

public class StockDataHub : Hub
{
    private readonly IYahooService _yahooService;

    public StockDataHub(IYahooService yahooService)
    {
        _yahooService = yahooService;
    }

    public ChannelReader<StockOutput> SendStock(string ticker, string start, string end, int updateInterval,
        CancellationToken token)
    {
        var channel = Channel.CreateUnbounded<StockOutput>();
        _ = GetStock(channel.Writer, ticker, start, end, updateInterval, token);
        return channel.Reader;
    }

    private async Task GetStock(ChannelWriter<StockOutput> writer, string ticker, string start, string end,
        int updateInterval, CancellationToken token)
    {
        var stocks = _yahooService.GetYahooStockEodData(ticker, DateTime.Parse(start), DateTime.Parse(end));

        var res = new StockOutput
        {
            Status = "Starting",
            TotalDataPoints = stocks.Count()
        };

        foreach (var stock in stocks)
        {
            token.ThrowIfCancellationRequested();
            res.Stock = stock;
            await writer.WriteAsync(res, token);
            await Task.Delay(updateInterval, token);
        }

        res.Status = "Finished";
        writer.TryComplete();
    }
}