namespace CarpetBG.Domain.Entities;
public class Product : BaseEntity
{
    public decimal Price { get; set; }
    public decimal ExpressServicePrice { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderBy { get; set; }
    public virtual List<OrderItem> OrderItems { get; set; } = [];

}
