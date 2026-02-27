using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using app;
using Blazored.SessionStorage;
using app.Services;
using FluentValidation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();
