using HackerNews.Api.Middleware;
using HackerNews.Application.AutoMapper;
using HackerNews.Infrastructure.Abstractions;
using HackerNews.Infrastructure.ExternalServices;
using HackerNews.Infrastructure.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
const string APPLICATION_ASSEMBLY_NAME = "HackerNews.Application";
const string corsPolicyName = "CorsPolicy";
const string localhostClient = "http://localhost:4200";

var applicationAssembly = AppDomain.CurrentDomain.Load(APPLICATION_ASSEMBLY_NAME);

// Add services to the container.

builder.Services.AddHttpClient();

// CORS configuration 

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        builder => builder.WithOrigins(localhostClient).SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddControllers();
builder.Services.AddMemoryCache(); // Add cache service in memory

// Add AutoMapperConfig as a singleton service
builder.Services.AddSingleton(AutoMapperConfig.Initialize());
builder.Services.AddScoped<ICacheService,CacheService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IHackerNewsAPIService, HackerNewsAPIService>();
builder.Services.AddTransient<IHttpClientFactoryWrapper, HttpClientFactoryWrapper>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(corsPolicyName);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();