// <copyright file="IIsdaService.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Services;

using Models.ISDA;

public interface IIsdaService
{
    /// <summary>
    ///     Gets the ISDA rates for a given currency.
    /// </summary>
    /// <param name="currency">The currency to check.</param>
    /// <param name="date">The date for the check.</param>
    /// <returns>A collection of IsdaRate objects.</returns>
    Task<IEnumerable<IsdaRate>> GetIsdaRates(string currency, string date);
}