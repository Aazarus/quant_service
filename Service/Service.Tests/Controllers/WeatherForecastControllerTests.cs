// <copyright file="WeatherForecastControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Service.Controllers;
using Xunit;

public class WeatherForecastControllerTests
{
    private readonly WeatherForecastController controller;
    private readonly Mock<ILogger<WeatherForecastController>> logger;

    public WeatherForecastControllerTests()
    {
        logger = new Mock<ILogger<WeatherForecastController>>();
        controller = new WeatherForecastController(logger.Object);
    }

    [Fact]
    public void Get_ShouldReturnACollectionOfWeatherForecastObjects()
    {
        // Arrange
        // Act
        var actual = controller.Get();

        // Assert
        actual.Should().NotBeNull();
        actual.Should().NotBeEmpty();
        actual.Count().Should().Be(5);
    }
}