using CardPrinter.DLogger;
using CardPrinter.Scryfall.Api.Models;
using NLog;
using System.Text.Json;

namespace CardPrinter.Scryfall.Api
{
    public class ApiManager
    {
        private readonly HttpClient _httpClient = new();
        public async Task<string> CallApiAsync(string url, ApiActionType action = ApiActionType.GET, HttpContent? content = null, bool formatString = false)
        {
            if (!IsValidUrl(url)) throw new ArgumentException("Url is not valid.");

            try
            {
                HttpResponseMessage? response = null;
                switch (action)
                {
                    case ApiActionType.GET:
                        response = await _httpClient.GetAsync(url);
                        break;
                    case ApiActionType.POST:
                        if (content == null) throw new ArgumentNullException(nameof(content));
                        response = await _httpClient.PostAsync(url, content);
                        break;
                    case ApiActionType.PUT:
                        if (content == null) throw new ArgumentNullException(nameof(content));
                        response = await _httpClient.PutAsync(url, content);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (response?.IsSuccessStatusCode == true)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    if (formatString)
                    {
                        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                        json = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                    }

                    return json;
                }
                else
                {
                    Log.TraceLine(LogLevel.Warn, "Response was unsuccessful: " + response?.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }

            return string.Empty;
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
        }
    }
}
