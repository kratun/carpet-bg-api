namespace CarpetBG.Application.DTOs.Orders;

public class PreLogisticSetupDto
{
    public List<Guid> OrderIds { get; set; } = [];
    public DateTime Date { get; set; }
}
