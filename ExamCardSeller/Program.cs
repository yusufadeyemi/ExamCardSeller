using ExamCardSeller.Infrastructure.Gateways;
using ExamCardSeller.Infrastructure.Persistence;
using ExamCardSeller.Infrastructure.Persistence.Repositories;
using ExamCardSeller.ServiceModels;
using ExamCardSeller.Services;
using ExamCardSeller.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using ExamCardSeller.Extensions;
using ExamCardSeller.Middlewares;
using Sentry;
using ExamCardSeller.AuthHandler;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPurchaseRequestRepository, PurchaseRequestRepository>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<PaystackService, PaystackService>();
var paystackSettings = builder.Configuration.GetSection("PaystackSettings").Get<PaystackSettings>();
builder.Services.AddSingleton<PaystackSettings>(paystackSettings!);
var appUser = builder.Configuration.GetSection("AppUser").Get<AppSecret>();
builder.Services.AddSingleton<AppSecret>(appUser!);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// Register FluentValidation validators
builder.Services.AddTransient<IValidator<CreateVerificationRequest>, CreatePurchaseRequestValidator>();
if (builder.Environment.IsProduction())
{
    builder.WebHost.UseSentry();
}
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.EnsureDatabaseSetup(true);

app.Run();
