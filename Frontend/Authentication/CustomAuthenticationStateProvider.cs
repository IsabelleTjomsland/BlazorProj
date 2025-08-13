using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace Frontend.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("jwtToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorageService.SetItemAsync("jwtToken", token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("jwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            try
            {
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                foreach (var kvp in keyValuePairs)
                {
                    var keyLower = kvp.Key.ToLower();

                    if (keyLower == "role" || keyLower == "roles" || keyLower == "systemrole")
                    {
                        if (kvp.Value is JsonElement roleElement)
                        {
                            if (roleElement.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var role in roleElement.EnumerateArray())
                                {
                                    if (!string.IsNullOrWhiteSpace(role.GetString()))
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, role.GetString().ToLower()));
                                    }
                                }
                            }
                            else if (roleElement.ValueKind == JsonValueKind.String)
                            {
                                var roleValue = roleElement.GetString();
                                if (!string.IsNullOrWhiteSpace(roleValue))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, roleValue.ToLower()));
                                }
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString().ToLower()));
                        }
                    }
                    else if (keyLower == "name")
                    {
                        claims.Add(new Claim(ClaimTypes.Name, kvp.Value.ToString()));
                    }
                    else if (keyLower == "email")
                    {
                        claims.Add(new Claim(ClaimTypes.Email, kvp.Value.ToString()));
                    }
                    else if (keyLower == "sub")
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, kvp.Value.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                    }
                }
            }
            catch
            {
                // Om JWT är ogiltig eller felaktigt formaterad
                claims.Add(new Claim(ClaimTypes.Role, ""));
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                case 1: base64 += "==="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
