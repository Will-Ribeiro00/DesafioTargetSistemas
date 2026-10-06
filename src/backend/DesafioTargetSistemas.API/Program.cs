using DesafioTargetSistemas.API.Filters;
using DesafioTargetSistemas.API.Middleware.Culture;
using DesafioTargetSistemas.Application;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Infrastructure;
using DesafioTargetSistemas.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(options => options.Filters.Add<ExceptionFilter>())
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = _ =>
        new BadRequestObjectResult(new ResponseErrorJson(ResourceMessageException.INVALID_REQUEST));
});

builder.Services.Configure<CultureSettings>(
    builder.Configuration.GetSection("Settings:Localization"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<CultureMiddleware>();

app.MapControllers();

app.Run();
