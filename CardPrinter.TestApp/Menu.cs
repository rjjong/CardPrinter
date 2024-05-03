using CardPinter.Core;
using CardPinter.DLogger;
using CardPinter.PersistanceLayer.Helpers;
using NLog;
using SixLabors.ImageSharp;

namespace CardPinter.TestApp;

public class Menu
{
    public static async Task Main(string[] args)
    {
        while (true)
        {
            switch (MenuOptions())
            {
                case ConsoleKey.S:
                    {
                        Console.Write("Enter a name to search for: ");
                        string? name = Console.ReadLine();
                        var cards = await CardManager.SearchCardAsync(name);
                        if (cards.Any())
                        {
                            foreach (var card in cards)
                            {
                                Console.WriteLine(string.Format("Id: {0,-8} | Name: {1, -40} | Type: {2}", card.Id, card.Name, string.Join(", ", card.CardDetails.Select(c => c.CardType))));
                                Log.TraceLine(LogLevel.Debug, string.Format("Id: {0,-8} | Name: {1, -40} | Type: {2}", card.Id, card.Name, string.Join(", ", card.CardDetails.Select(c => c.CardType))));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No cards found.");
                        }
                        Console.ReadKey();
                    }
                    break;
                case ConsoleKey.R:
                    {
                        using (var cardRepository = new CardRepository())
                        {
                            var card = await cardRepository.GetRandomCardAsync();
                            Console.WriteLine(string.Format("{0}, {1}", card.Name, card.CardImages?.FirstOrDefault()?.ImageUri));
                            Log.TraceLine(LogLevel.Debug, "Random card: " + card.Name);
                        }
                        Console.ReadKey();
                    }
                    break;
                case ConsoleKey.I:
                    {
                        Console.Write("Enter a path for a json file to import: ");
                        string? path = Console.ReadLine();
                        if (path != null && path.ToLower() != "q")
                        {
                            Console.WriteLine("Clearing database...");
                            await CardManager.ClearDatabase();
                            Console.WriteLine("Importing card data...");
                            await CardManager.ImportCardsAsync(path);
                            Console.WriteLine("Import complete.");
                            Log.TraceLine(LogLevel.Debug, "Database was recreated.");
                        }
                        else
                        {
                            Console.WriteLine("Operation canceled.");
                        }
                        Console.ReadKey();
                    }
                    break;
                case ConsoleKey.G:
                    try
                    {
                        Console.Write("Enter an Card.Id for the card to download an convert to grayscale: ");
                        int cardId = TryParseInt();
                        var cardImage = (await CardManager.GetCardAsync(cardId))?.CardImages?.FirstOrDefault();
                        if (cardImage != null)
                        {
                            Console.Write("Enter destination directory of image: ");
                            string? path = Console.ReadLine();

                            byte[] image = await CardManager.DownloadImageAsync(cardImage.ImageUri);
                            await CardManager.SaveAsGrayscaleAsync(image, path);
                            Console.WriteLine("Image saved successfully.");
                            Log.TraceLine(LogLevel.Debug, "Image saved successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Cannot find card with that Id.");
                        }
                        await Task.Delay(2000);
                        
                    }
                    catch (OperationCanceledException) 
                    {
                    
                    }
                    break;
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    return;
                default:
                    Console.WriteLine("Key not valid."); 
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static int TryParseInt()
    {
        int result = 0;
        string? input = Console.ReadLine();
        if (input?.ToLower() == "q")
            throw new OperationCanceledException("Operation canceled by the user.");

        while (!int.TryParse(input, out result))
        {
            Console.Write("That is not a number. Please try again: ");
            input = Console.ReadLine();
            if (input?.ToLower() == "q") 
                throw new OperationCanceledException("Operation canceled by the user.");
        }

        return result;
    }

    private static ConsoleKey MenuOptions()
    {
        Console.Clear();
        Console.WriteLine("This is a test application for the CardPrinter project.");
        Console.WriteLine("Press any of the following keys:");
        Console.WriteLine("  R: Get random card");
        Console.WriteLine("  S: Search for a card name");
        Console.WriteLine("  G: Convert Card.Id to grayscale");
        Console.WriteLine("  I: Clean database and import a new card data file (json)");
        Console.WriteLine("  Q: Quit application or cancel operation");

        var key = Console.ReadKey().Key;
        Console.Clear();
        return key;
    }
}