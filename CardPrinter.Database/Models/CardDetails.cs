using System.ComponentModel.DataAnnotations.Schema;

namespace CardPrinter.Database.Models;

[Table(nameof(CardDetails))]
public class CardDetails : BaseModel
{
    public string? ManaCost { get; set; }
    public string? CardType { get; set; }
    public string? OracleText { get; set; }

    [ForeignKey(nameof(CardInfo))]
    public int CardInfoId { get; set; }
    public virtual CardInfo? CardInfo { get; set; }
}
