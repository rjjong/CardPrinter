using CardPinter.Database.Models;
using CardPinter.DLogger;
using CardPinter.PersistanceLayer.Helpers;
using NLog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace CardPinter.Core
{
    public static class CardManager
    {
        public static Task ClearDatabase()
        {
            using (var cardRepository = new CardRepository())
            {
                return cardRepository.ClearDatabase();
            }
        }

        public static async Task SaveAsGrayscaleAsync(byte[] imageData, string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new MemoryStream(imageData))
                using (var image = await Image.LoadAsync<Rgba32>(stream))
                {
                    image.Mutate(x => x.Grayscale());
                    await image.SaveAsync(Path.Combine(directory, "gray_card.png"), new PngEncoder());
                }
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }
        }

        public static async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await stream.CopyToAsync(memoryStream);
                                    return memoryStream.ToArray();
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to download image. Status code: {response.StatusCode}");
                            return [];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return [];
            }
        }

        public static async Task<CardInfo?> GetCardAsync(int cardId)
        {
            try
            {
                using (var cardRepository = new CardRepository())
                {
                    return await cardRepository.GetCardAsync(cardId);
                }
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }

            return null;
        }

        public static async Task ImportCardsAsync(string path)
        {
            try
            {
                string jsonText = File.ReadAllText(path);
                var cards = CardDeserializer.CardsFromJson(jsonText);

                using (var cardRepository = new CardRepository())
                {
                    await cardRepository.AddCardsAsync(cards);
                }
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }
        }

        public static async Task<List<CardInfo>> SearchCardAsync(string? searchString)
        {
            try
            {
                Regex regex = new Regex(@"^(.*?)(?:\s*-t:(.*?))?(?:\s*-o:(.*?))?$");
                Match match = regex.Match(searchString);

                if (match.Success)
                {
                    using (var cardsRepository = new CardRepository())
                    {
                        string searchTerm = match.Groups[1].Value.Trim();

                        var cards = await cardsRepository.FindCards(searchTerm);
                        if (match.Groups[2].Success)
                        {
                            string searchType = match.Groups[2].Value.Trim().ToLower();
                            cards = cards.Where(c => c.CardDetails.Any(d => d.CardType?.ToLower().Contains(searchType) ?? false)).ToList();
                        }
                        if (match.Groups[3].Success)
                        {
                            string searchOracle = match.Groups[3].Value.Trim().ToLower();
                            cards = cards.Where(c => c.CardDetails.Any(d => d.OracleText?.ToLower().Contains(searchOracle) ?? false)).ToList();
                        }

                        return cards;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }

            return new List<CardInfo>();
        }
    }
}
