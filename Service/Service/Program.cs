// <copyright file="Program.cs" company="Sevna Software LTD">
// Copyright (c) Sevna Software LTD. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Service.Models;
using Service.Services;
using Service.Wrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<QuantDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["Data:Quant:ConnectionString"]);
});

builder.Services.AddSingleton<IYahooQuotesApiWrapper, YahooQuotesApiWrapper>();
builder.Services.AddSingleton<IYahooService, YahooService>();
builder.Services.AddSingleton<IIEXService, IEXService>();
builder.Services.AddSingleton<IIEXApiWrapper, IEXApiWrapper>();

// API Keys
builder.Services.AddSingleton(
    new ApiKeySettings.IEX
    {
        PublishableToken = builder.Configuration.GetSection("ApiKeys:IEX:PublishableToken").Value!,
        SecurityToken = builder.Configuration.GetSection("ApiKeys:IEX:SecurityToken").Value!
    }
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowOrigin",
        b =>
        {
            b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowOrigin");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<QuantDataContext>();
DbInitialiser.Initialise(dbContext);

app.Run();