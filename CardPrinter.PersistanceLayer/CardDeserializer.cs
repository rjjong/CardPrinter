using CardPinter.Database.Models;
using CardPinter.DLogger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.Collections.Concurrent;

namespace CardPrinter.PersistanceLayer
{
    public static class CardDeserializer
    {
        public static CardInfo? CardFromJson(string jsonString)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);

                CardInfo cardInfo = new CardInfo
                {
                    CardId = Guid.Parse(jsonObject.GetValue("id")?.ToString()),
                    Name = jsonObject.GetValue("name")?.ToString() ?? string.Empty,
                    ReleaseDate = DateTime.Parse(jsonObject.GetValue("released_at")?.ToString()),
                    CardDetails = new List<CardDetails>()
                    {
                        new CardDetails
                        {
                            ManaCost = jsonObject.GetValue("mana_cost")?.ToString(),
                            CardType = jsonObject.GetValue("type_line")?.ToString(),
                            OracleText = jsonObject.GetValue("oracle_text")?.ToString()
                        }
                    },
                };

                var imageUrisToken = jsonObject.GetValue("image_uris");
                if (imageUrisToken != null)
                {
                    if (imageUrisToken.Type == JTokenType.Array)
                    {
                        var imageUrisArray = (JArray)imageUrisToken;
                        foreach (var imageUriToken in imageUrisArray)
                        {
                            var imageUri = imageUriToken["uri"]?.ToString();
                            if (imageUri != null)
                            {
                                string? imageType = imageUriToken["type"]?.ToString();

                                if (cardInfo.CardImages == null)
                                    cardInfo.CardImages = new List<CardImageInfo>();

                                cardInfo.CardImages.Add(new CardImageInfo
                                {
                                    ImageType = imageType,
                                    ImageUri = imageUri
                                });
                            }
                        }
                    }
                    else if (imageUrisToken.Type == JTokenType.Object)
                    {
                        var imageUrisObject = (JObject)imageUrisToken;
                        cardInfo.CardImages = new List<CardImageInfo>
                        {
                            new CardImageInfo
                            {
                                ImageType = "normal",
                                ImageUri = imageUrisObject.GetValue("normal")?.ToString()
                            },
                            new CardImageInfo
                            {
                                ImageType = "large",
                                ImageUri = imageUrisObject.GetValue("large")?.ToString()
                            }
                        };
                    }
                }

                return cardInfo;
            }
            catch (Exception ex)
            {
                Log.TraceLine(LogLevel.Error, ex.Message);
            }

            return null;
        }

        public static List<CardInfo> CardsFromJson(string jsonText)
        {
            var objectList = JsonConvert.DeserializeObject<List<object>>(jsonText);

            var cards = new ConcurrentBag<CardInfo>();
            Parallel.ForEach(objectList, json =>
            {
                cards.Add(CardFromJson(json.ToString()));
            });

            return cards.ToList();
        }
    }
}
