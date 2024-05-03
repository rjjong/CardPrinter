using System.ComponentModel.DataAnnotations.Schema;

namespace CardPrinter.Database.Models;

[Table(nameof(CardImageInfo))]
public class CardImageInfo : BaseModel
{
    public string? ImageType { get; set; }
    public string? ImageUri { get; set; }

    [ForeignKey(nameof(CardInfo))]
    public int CardInfoId { get; set; }
    public virtual CardInfo? CardInfo { get; set; }
}
