using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.AspNetCore;

using PaymentGateway.Api.ErrorHandler;
using PaymentGateway.Application.BankSimulator;
using PaymentGateway.Application.Repository;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models.Requests;
using PaymentGateway.Domain.Validators;
using PaymentGateway.Infrastructure;
using PaymentGateway.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

IWebHostEnvironment environment = builder.Environment;

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.UseInlineDefinitionsForEnums();
    option.SwaggerDoc("v1", new() { Title = "PaymentGateway.Api", Version = "v1", Description = "REST API to manage payment gateway" });
    option.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
    option.UseInlineDefinitionsForEnums();
});

builder.Services.AddMemoryCache();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSingleton<IPaymentRepository, PaymentsRepository>();
builder.Services.AddTransient<IPaymentServices, PaymentServices>();
builder.Services.AddHttpClient<IBankAdapter, BankAdapter>();

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

app.UseExceptionMiddleware();

app.Run();
