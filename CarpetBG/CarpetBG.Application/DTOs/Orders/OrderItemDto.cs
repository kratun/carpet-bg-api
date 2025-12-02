namespace CarpetBG.Application.DTOs.Orders;

public class OrderItemDto
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public decimal? Diagonal { get; set; }
    public decimal Price { get; set; }
    public string Note { get; set; } = string.Empty;
    public List<Guid> Additions { get; set; } = [];
}
