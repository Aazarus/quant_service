// <copyright file="StockValuesControllerTests.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Tests.Controllers;

using System.Collections.Generic;
using System.Linq;
using Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using Service.Controllers;
using Xunit;

public class StockValuesControllerTests
{
    private readonly StockValuesController _controller;
    private readonly Mock<ILogger<StockValuesController>> _logger;
    private IEnumerable<IndexData>? _indexData;
    private Mock<QuantDataContext>? _mockDbContext;
    private IEnumerable<Price>? _prices;
    private IEnumerable<Symbol>? _symbols;

    public StockValuesControllerTests()
    {
        SetupDatabaseMocks();
        _logger = new Mock<ILogger<StockValuesController>>();
        _controller = new StockValuesController(_mockDbContext!.Object, _logger.Object);
    }

    [Fact]
    public void GetSymbols_ShouldReturnTheFullCollection()
    {
        // Arrange
        // Act
        var actual = _controller.GetSymbols();

        // Assert
        actual.Should().BeEquivalentTo(_symbols);
    }

    private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
    {
        var elementsAsQueryable = elements.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

        return dbSetMock;
    }

    private void SetupDatabaseMocks()
    {
        _symbols = TestData.Symbols;
        _indexData = TestData.IndexData;
        _prices = TestData.Prices;
        var mockSymbols = CreateDbSetMock(_symbols);
        var mockIndexData = CreateDbSetMock(_indexData);
        var mockPrices = CreateDbSetMock(_prices);
        _mockDbContext = new Mock<QuantDataContext>();
        _mockDbContext.Setup(x => x.Symbols).Returns(mockSymbols.Object);
        _mockDbContext.Setup(x => x.IndexData).Returns(mockIndexData.Object);
        _mockDbContext.Setup(x => x.Prices).Returns(mockPrices.Object);
    }
}