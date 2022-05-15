// <copyright file="QuantDataContext.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

namespace Service.Models;

using Microsoft.EntityFrameworkCore;

public class QuantDataContext : DbContext
{
    public QuantDataContext(DbContextOptions<QuantDataContext> options)
        : base(options)
    {
    }

    public DbSet<Symbol>? Symbols { get; set; }
    public DbSet<Price>? Prices { get; set; }
    public DbSet<IndexData>? IndexData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Symbol>().ToTable("Symbol");
        modelBuilder.Entity<Price>().ToTable("Price");
        modelBuilder.Entity<IndexData>().ToTable("IndexData");
    }
}