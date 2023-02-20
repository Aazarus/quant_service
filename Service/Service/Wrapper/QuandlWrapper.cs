// <copyright file="QuandlWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using Quandl.NET;
using Quandl.NET.Model.Response;

public class QuandlWrapper : IQuandlWrapper
{
    /// <inheritdoc />
    public async Task<TimeseriesDataResponse> GetQuandlStock(string apiKey, string ticker, string start, string end)
    {
        var startDate = DateTime.Parse(start);
        var endDate = DateTime.Parse(end);
        var client = new QuandlClient(apiKey);

        return await client.Timeseries.GetDataAsync("WIKI", ticker, startDate: startDate, endDate: endDate);
    }
}