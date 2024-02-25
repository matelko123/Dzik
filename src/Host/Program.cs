using Application;
using Host.Endpoints.Internal;
using Host.Middleware;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


var app = builder.Build();

await app.Services.InitializeDatabasesAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseInfrastructure();
app.MapInfrastructureEndpoints();
app.UseEndpoints<Program>();
// app.MapIdentityApi<AppUser>();
app.Run();

public partial class Program;