using ImageDetectionTests.Client;
using ImageDetectionTests.Client.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<IImageDataHandler, ImageDataHandler>();
builder.Services.AddTransient<IOpenCvInterop, OpenCvInterop>();
builder.Services.AddBlazorJSRuntime();

await builder.Build().RunAsync();