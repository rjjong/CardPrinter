# Card Printer Test Application

The Card Printer project includes components for importing generic MTG cards in a local database, quering from it and downloading images from a sryfall url.
The code will later on serve as software for a thermal printer device that is able to print any card that is requested by the user.
I got the idea from: "https://github.com/oboyone/mb_thermal_printer" and wanted to implement it in my own way.

## Installation

To run the application, follow these steps:

1. Clone this repository.
2. Navigate to the project directory.
4. Set the TestApp as startup if it wasn't already.
3. Build the project using the .NET Core CLI:
5. Run the application:

## Usage

The test console application provides several options accessible via keyboard input:

- **R:** Get a random card from the database.
- **S:** Search for a card by name. Input parameters can be given to also search for type (-t:<type>) or characters in the oracle text (-o:<oracle>).
- **G:** Download and convert a card's image to grayscale and save it.
- **I:** Clean the database and import new card data from a JSON file.
- **Q:** Quit the application or cancel the current operation if possible.

## Dependencies

The application relies on the following libraries:

- `NLog`
- `SixLabors.ImageSharp`

## Contributing

Contributions are welcome. If you encounter any issues or have suggestions for improvement, please open an issue or submit a pull request on GitHub.
