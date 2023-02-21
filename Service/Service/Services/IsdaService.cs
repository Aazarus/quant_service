// <copyright file="IsdaService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models.ISDA;
using Wrapper;

public class IsdaService : IIsdaService
{
    /// <summary>
    ///     The ISDA wrapper.
    /// </summary>
    private readonly IIsdaWrapper _apiWrapper;

    /// <summary>
    ///     The application logger.
    /// </summary>
    private readonly ILogger<IsdaService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IsdaService" /> class.
    /// </summary>
    /// <param name="logger">The Application Logger.</param>
    /// <param name="apiWrapper">The ISDA Api Wrapper.</param>
    public IsdaService(ILogger<IsdaService> logger, IIsdaWrapper apiWrapper)
    {
        _logger = logger;
        _apiWrapper = apiWrapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IsdaRate>> GetIsdaRates(string currency, string date)
    {
        return await _apiWrapper.GetIsdaRates(currency, date);
    }
}