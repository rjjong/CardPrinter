using CardPinter.Database;
using CardPinter.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CardPrinter.PersistanceLayer
{
    public class CardRepository : LocalRepositoryBase
    {
        public CardRepository() : base()
        {

        }

        public async Task AddCardsAsync(IEnumerable<CardInfo> cards)
        {
            await DatabaseContext.AddRangeAsync(cards);
            await DatabaseContext.SaveChangesAsync();
        }

        public async Task ClearDatabase()
        {
            await DatabaseContext.Database.ExecuteSqlRawAsync("DELETE FROM CardInfo");
            await DatabaseContext.SaveChangesAsync();
        }

        public Task<List<CardInfo>> FindCards(string name)
        {
            return DatabaseContext.Cards.Include(c => c.CardDetails).Where(c => c.Name.Contains(name)).ToListAsync();
        }

        public Task<CardInfo?> GetCardAsync(int cardId)
        {
            return DatabaseContext.Cards
                .Include(c => c.CardImages)
                .Include(c => c.CardDetails)
                .Where(c => c.Id == cardId)
                .FirstOrDefaultAsync();
        }

        public Task<CardInfo> GetRandomCardAsync()
        {
            return DatabaseContext.Cards
                .FromSqlRaw("SELECT * FROM CardInfo ORDER BY RANDOM() LIMIT 1")
                .Include(c => c.CardImages)
                .Include(c => c.CardDetails)
                .FirstAsync();
        }
    }
}
