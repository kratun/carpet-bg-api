namespace CarpetBG.Domain.Entities;

public class OrderItem : BaseEntity
{
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public decimal? Diagonal { get; set; }
    public decimal Price { get; set; }
    public decimal ExpressServicePrice { get; set; }
    public string Note { get; set; } = string.Empty;
    public List<OrderItemAddition> Additions { get; set; } = [];
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public Guid? ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}
