using Blazor;
using Client.Infrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args)
    .AddClientServices();

builder.RootComponents.Add<App>("#app");
builder.Services.AddMudServices();

await builder.Build().RunAsync();
