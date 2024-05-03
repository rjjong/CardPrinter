using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardPinter.Database.Models;

[Table(nameof(CardInfo))]
public class CardInfo : BaseModel
{
    public CardInfo()
    {
        CardId = Guid.NewGuid();
        Name = string.Empty;
    }

    public required Guid CardId { get; set; }
    public required string Name { get; set; }
    public DateTime? ReleaseDate { get; set; }

    public virtual ICollection<CardDetails>? CardDetails { get; set; }
    public virtual ICollection<CardImageInfo>? CardImages { get; set; }
}
