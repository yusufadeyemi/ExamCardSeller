using ExamCardSeller.Infrastructure.Gateways;
using ExamCardSeller.Infrastructure.Persistence;
using ExamCardSeller.Infrastructure.Persistence.Repositories;
using ExamCardSeller.ServiceModels;
using ExamCardSeller.Services;
using ExamCardSeller.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<PaystackService, PaystackService>();
var paystackSettings = builder.Configuration.GetSection("PaystackSettings").Get<PaystackSettings>();
builder.Services.AddSingleton<PaystackSettings>(paystackSettings!);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// Register FluentValidation validators
builder.Services.AddTransient<IValidator<CreateVerificationRequest>, CreatePurchaseRequestValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
