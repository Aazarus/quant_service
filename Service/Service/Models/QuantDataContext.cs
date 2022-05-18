// <copyright file="QuantDataContext.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public class QuantDataContext : DbContext
{
    protected QuantDataContext()
    {
    }

    public QuantDataContext(DbContextOptions<QuantDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Symbol>? Symbols { get; set; }
    public virtual DbSet<Price>? Prices { get; set; }
    public virtual DbSet<IndexData>? IndexData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Symbol>().ToTable("Symbol");
        modelBuilder.Entity<Price>().ToTable("Price");
        modelBuilder.Entity<IndexData>().ToTable("IndexData");
    }
}