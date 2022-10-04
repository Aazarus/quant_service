// <copyright file="AlphaVantageService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models;
using Models.AlphaVantage;
using Newtonsoft.Json;
using Wrapper;

public class AlphaVantageService : IAlphaVantageService
{
    /// <summary>
    ///     The API Keys.
    /// </summary>
    private readonly ApiKeySettings.AlphaVantage _apiKey;

    /// <summary>
    ///     The IEX Api Wrapper.
    /// </summary>
    private readonly IAlphaVantageWrapper _apiWrapper;

    /// <summary>
    ///     The Application Logger.
    /// </summary>
    private readonly ILogger<AlphaVantageService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AlphaVantageService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="apiKey">The AlphaVantage Api Keys.</param>
    /// <param name="apiWrapper">The AlphaVantage Wrapper.</param>
    public AlphaVantageService(ILogger<AlphaVantageService> logger, ApiKeySettings.AlphaVantage apiKey,
        IAlphaVantageWrapper apiWrapper)
    {
        _logger = logger;
        _apiKey = apiKey;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<List<StockData>> GetStockEOD(string ticker, string start, string end, string period)
    {
        string response = await _apiWrapper.GetStockEOD(ticker, start, period, _apiKey.ApiKey);
        return ProcessEODResponseForStockData(ticker, response, start, end).OrderBy(d => d.Date).ToList();
    }

    /// <inheritdoc />
    public async Task<List<StockData>> GetStockBar(string ticker, int interval, int outputSize)
    {
        string response = await _apiWrapper.GetStockBar(ticker, interval, outputSize, _apiKey.ApiKey);
        return ProcessBarResponseForStockData(ticker, response).OrderBy(d => d.Date).ToList();
    }

    /// <inheritdoc />
    public async Task<AvStockQuote> GetStockQuote(string ticker)
    {
        string response = await _apiWrapper.GetStockQuote(ticker, _apiKey.ApiKey);
        return ProcessQuoteResponseForAvStockQuote(response);
    }

    /// <inheritdoc />
    public async Task<List<AvFxData>> GetFxEOD(string ticker, string start, string period)
    {
        string response = await _apiWrapper.GetFxEOD(ticker, start, period, _apiKey.ApiKey);
        return ProcessFxEODDataResponseForAvFxData(ticker, response);
    }

    /// <inheritdoc />
    public async Task<List<AvFxData>> GetFxBar(string ticker, int interval, int outputsize)
    {
        string response = await _apiWrapper.GetFxBar(ticker, interval, outputsize, _apiKey.ApiKey);
        return ProcessFxBarDataResponseForAvFxData(ticker, response);
    }

    /// <inheritdoc />
    public async Task<List<AvSectorPref>> GetSectorPref()
    {
        string response = await _apiWrapper.GetSectorPref(_apiKey.ApiKey);
        return ProcessSectorPerformanceForAvSectorPref(response);
    }

    private IEnumerable<StockData> ProcessEODResponseForStockData(string ticker, string response, string start,
        string end)
    {
        var models = new List<StockData>();
        if (ConfirmResponseIsValid(response))
        {
            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    var date = DateTime.Parse(r[0]);

                    if (date >= startDate && date <= endDate)
                        models.Add(new StockData
                        {
                            Ticker = ticker,
                            Date = date,
                            Open = decimal.Parse(r[1]),
                            High = decimal.Parse(r[2]),
                            Low = decimal.Parse(r[3]),
                            Close = decimal.Parse(r[4]),
                            CloseAdj = decimal.Parse(r[5]),
                            Volume = decimal.Parse(r[6])
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }

        return models;
    }

    private IEnumerable<StockData> ProcessBarResponseForStockData(string ticker, string response)
    {
        var models = new List<StockData>();

        if (ConfirmResponseIsValid(response))
        {
            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    var date = DateTime.Parse(r[0]);
                    models.Add(new StockData
                    {
                        Ticker = ticker,
                        Date = date,
                        Open = decimal.Parse(r[1]),
                        High = decimal.Parse(r[2]),
                        Low = decimal.Parse(r[3]),
                        Close = decimal.Parse(r[4]),
                        Volume = decimal.Parse(r[5])
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }

        return models;
    }

    private AvStockQuote ProcessQuoteResponseForAvStockQuote(string response)
    {
        if (ConfirmResponseIsValid(response))
        {
            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    return new AvStockQuote
                    {
                        Ticker = r[0],
                        TimeStamp = DateTime.Now,
                        Open = decimal.Parse(r[1]),
                        High = decimal.Parse(r[2]),
                        Low = decimal.Parse(r[3]),
                        Price = decimal.Parse(r[4]),
                        Volume = decimal.Parse(r[5]),
                        PrevClose = decimal.Parse(r[7]),
                        Change = decimal.Parse(r[8]),
                        ChangePercent = decimal.Parse(r[9].TrimEnd('%', ' ')) / 100m
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }

        return new AvStockQuote();
    }

    private List<AvFxData> ProcessFxEODDataResponseForAvFxData(string ticker, string response)
    {
        if (ConfirmResponseIsValid(response))
        {
            ticker = AlphaVantageWrapper.SanitiseFxTicker(ticker);
            var models = new List<AvFxData>();

            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    var date = DateTime.Parse(r[0]);
                    models.Add(new AvFxData
                    {
                        Ticker = ticker,
                        Date = date,
                        Open = decimal.Parse(r[1]),
                        High = decimal.Parse(r[2]),
                        Low = decimal.Parse(r[3]),
                        Close = decimal.Parse(r[4])
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return models.OrderBy(d => d.Date).ToList();
        }

        return new List<AvFxData>();
    }

    private List<AvFxData> ProcessFxBarDataResponseForAvFxData(string ticker, string response)
    {
        if (ConfirmResponseIsValid(response))
        {
            ticker = AlphaVantageWrapper.SanitiseFxTicker(ticker);
            var models = new List<AvFxData>();

            response = response.Replace("\r", "");
            string[] rows = response.Split("\n");

            foreach (string row in rows.Skip(1))
            {
                string[] r = row.Split(",");

                try
                {
                    var date = DateTime.Parse(r[0]);
                    models.Add(new AvFxData
                    {
                        Ticker = ticker,
                        Date = date,
                        Open = decimal.Parse(r[1]),
                        High = decimal.Parse(r[2]),
                        Low = decimal.Parse(r[3]),
                        Close = decimal.Parse(r[4])
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }

            return models.OrderBy(d => d.Date).ToList();
        }

        return new List<AvFxData>();
    }

    private List<AvSectorPref> ProcessSectorPerformanceForAvSectorPref(string response)
    {
        if (ConfirmResponseIsValid(response))
        {
            var res = JsonConvert.DeserializeObject<dynamic>(response);
            var ranks = new[]
            {
                "Rank A: Real-Time Performance",
                "Rank B: 1 Day Performance",
                "Rank C: 5 Day Performance",
                "Rank D: 1 Month Performance",
                "Rank E: 3 Month Performance",
                "Rank F: Year-to-Date (YTD) Performance",
                "Rank G: 1 Year Performance",
                "Rank H: 3 Year Performance",
                "Rank I: 5 Year Performance",
                "Rank J: 10 Year Performance"
            };

            return (from rank in ranks
                where response.Contains(rank)
                select new AvSectorPref
                {
                    Rank = rank,
                    CommunicationServices = res[rank]["Communication Services"],
                    ConsumerDiscretionary = res[rank]["Consumer Discretionary"],
                    ConsumerStaples = res[rank]["Consumer Staples"],
                    Energy = res[rank]["Energy"],
                    Financials = res[rank]["Financials"],
                    HealthCare = res[rank]["Health Care"],
                    Industrials = res[rank]["Industrials"],
                    InformationTechnology = res[rank]["Information Technology"],
                    Materials = res[rank]["Materials"],
                    Utilities = res[rank]["Utilities"]
                }).ToList();
        }

        return new List<AvSectorPref>();
    }

    private bool ConfirmResponseIsValid(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            _logger.LogInformation("Response is invalid (either null, empty, or whitespace)");
            return false;
        }

        if (response.Contains("This is a premium endpoint"))
            throw new NotSupportedException("Request requires Premium subscription to AlphaVantage.");

        if (response.Contains("parameter apikey is invalid or missing"))
        {
            _logger.LogError("Error calling AlphaVantage. API key may be invalid.");
            throw new Exception("Issue getting data from AlphaVantage");
        }

        return true;
    }
}