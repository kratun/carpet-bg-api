namespace CarpetBG.Application.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public decimal ExpressServicePrice { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int OrderBy { get; set; }
}
