using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // ⬅️ Viktigt för AuthenticationStateProvider
using Frontend.Authentication;     // ⬅️ Se till att detta namespace stämmer
using Bemanning_System.Frontend.Services;           // ⬅️ Om du har AuthService/IAuthService här

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrera HTTP-klienten för backend-anrop
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5082/") });

// 🔐 Autentisering och lokal lagring
builder.Services.AddAuthorizationCore(); // Aktiverar [Authorize]
builder.Services.AddBlazoredLocalStorage(); // För att lagra JWT i localStorage

// AuthenticationStateProvider
builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // För direkt användning
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthenticationStateProvider>()); // För DI via interface

// ⬇️ Lägg till AuthService om du använder det
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
