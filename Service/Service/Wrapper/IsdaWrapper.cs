// <copyright file="IsdaWrapper.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Wrapper;

using System.IO.Compression;
using System.Xml.Linq;
using Models.ISDA;

public class IsdaWrapper : IIsdaWrapper
{
    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<IsdaWrapper> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IsdaWrapper" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    public IsdaWrapper(ILogger<IsdaWrapper> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IsdaRate>> GetIsdaRates(string currency, string date)
    {
        string url = $"https://www.markit.com/news/InterestRates_{currency.ToUpper()}_{date}.zip";

        var rates = new List<IsdaRate>();

        using var client = new HttpClient();
        try
        {
            byte[] data = await client.GetByteArrayAsync(url);
            var stream = new MemoryStream(data);
            using var archive = new ZipArchive(stream);

            foreach (var entry in archive.Entries)
                if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = XDocument.Load(entry.Open());
                    rates = ProcessIsdaRates(doc, currency);
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return rates;
    }

    private List<IsdaRate> ProcessIsdaRates(XDocument doc, string currency)
    {
        if (doc.Root == null) throw new NullReferenceException($"{nameof(doc)} is null.");

        var asof = doc.Root.Element(IsdaConsts.EffectiveAsOf);
        var badDay = doc.Root.Element(IsdaConsts.BadDayConvention);

        var rates = new List<IsdaRate>();
        rates.AddRange(ProcessDeposits(doc, currency, asof, badDay));
        rates.AddRange(ProcessSwaps(doc, currency, asof, badDay));

        return rates;
    }

    private IEnumerable<IsdaRate> ProcessSwaps(XDocument doc, string currency, XElement asof, XElement badday)
    {
        if (doc.Root == null) throw new NullReferenceException($"{nameof(doc)} is null.");
        var rates = new List<IsdaRate>();

        // Process swaps
        var swaps = doc.Root.Element(IsdaConsts.Swaps);

        if (swaps == null) throw new NullReferenceException($"{nameof(swaps)} is null.");

        try
        {
            string? dayConvention = swaps.Element(IsdaConsts.FloatingCountConvention)?.Value;
            string? calendar = swaps.Element(IsdaConsts.Calendars)?.Element(IsdaConsts.Calendar)?.Value;
            string? fixDayConvention = swaps.Element(IsdaConsts.FixedDayConvention)?.Value;
            string?[] ss = swaps.Element(IsdaConsts.SnapTime)?.Value.Split('T')!;
            string ts = ss[0] + " " + ss[1]!.Split('Z')[0];
            var snapDate = Convert.ToDateTime(ts);
            string? spotDate = swaps.Element(IsdaConsts.SnapTime)?.Value;
            string? floatPay = swaps.Element(IsdaConsts.FloatingPaymentFrequency)?.Value;
            string? fixPay = swaps.Element(IsdaConsts.FixedPaymentFrequency)?.Value;

            foreach (var element in swaps.Elements())
                if (element.Name.ToString() == IsdaConsts.CurvePoint)
                    try
                    {
                        rates.Add(new IsdaRate
                        {
                            Currency = currency,
                            EffectiveAsOf = asof.Value,
                            BadDayConvention = badday.Value,
                            Calendar = calendar,
                            SnapTime = snapDate,
                            SpotDate = spotDate,
                            Maturity = element.Element(IsdaConsts.MaturityDate)?.Value,
                            DayCountConvention = dayConvention,
                            FixedDayCountConvention = fixDayConvention,
                            FloatingPaymentFrequency = floatPay,
                            FixedPaymentFrequency = fixPay,
                            Tenor = element.Element(IsdaConsts.Tenor)?.Value,
                            Rate = element.Element(IsdaConsts.ParRate)?.Value
                        });
                    }
                    catch (NullReferenceException ex)
                    {
                        Console.WriteLine($"Null Reference Exception: {ex.Message}.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to process swaps. Message: {ex.Message}");
        }

        return rates;
    }

    private static IEnumerable<IsdaRate> ProcessDeposits(XDocument doc, string currency, XElement asof, XElement badday)
    {
        if (doc.Root == null) throw new NullReferenceException($"{nameof(doc)} is null.");
        var rates = new List<IsdaRate>();

        // Process Deposits
        var deposits = doc.Root.Element(IsdaConsts.Deposits);
        if (deposits == null) throw new NullReferenceException($"{nameof(deposits)} is null.");

        string dayConvention = deposits.Element(IsdaConsts.DayCountConvention)?.Value;
        string calendar = deposits.Element(IsdaConsts.Calendars)?.Element(IsdaConsts.Calendar)?.Value;
        string[] ss = deposits.Element(IsdaConsts.SnapTime)?.Value.Split('T');
        string timeStamp = ss[0] + " " + ss[1].Split('Z')[0];
        var snapDate = Convert.ToDateTime(timeStamp);
        string spotDate = deposits.Element(IsdaConsts.SpotDate)?.Value;

        foreach (var element in deposits.Elements())
            if (element.Name == IsdaConsts.CurvePoint)
                try
                {
                    rates.Add(new IsdaRate
                    {
                        Currency = currency,
                        EffectiveAsOf = asof.Value,
                        BadDayConvention = badday.Value,
                        Calendar = calendar,
                        SnapTime = snapDate,
                        SpotDate = spotDate,
                        Maturity = element.Element(IsdaConsts.MaturityDate)?.Value,
                        DayCountConvention = dayConvention,
                        Tenor = element.Element(IsdaConsts.Tenor)?.Value,
                        Rate = element.Element(IsdaConsts.ParRate)?.Value
                    });
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine($"Null Reference Exception: {ex.Message}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

        return rates;
    }
}