// <copyright file="ApiKeySettings.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ApiKeySettings
{
    /// <summary>
    ///     Defines the class holding the API Key for IEX access.
    /// </summary>
    public class IEX
    {
        /// <summary>
        ///     Gets or sets the Publishable Token for IEX access.
        /// </summary>
        public string PublishableToken { get; set; } = "";

        /// <summary>
        ///     Gets or sets the Security Token for IEX access.
        /// </summary>
        public string SecurityToken { get; set; } = "";
    }

    /// <summary>
    ///     Defines the class holding the API Key for AlphaVantage access.
    /// </summary>
    public class AlphaVantage
    {
        /// <summary>
        ///     Gets or sets the Api Key for AlphaVantage access.
        /// </summary>
        public string ApiKey { get; set; } = "";
    }
}