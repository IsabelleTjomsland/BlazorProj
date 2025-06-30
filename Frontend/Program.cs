using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Viktigt: BaseAddress för backend-API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5082/") });

await builder.Build().RunAsync();
