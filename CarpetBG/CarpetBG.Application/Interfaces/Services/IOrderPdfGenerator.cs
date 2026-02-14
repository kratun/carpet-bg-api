using CarpetBG.Application.DTOs.Orders;

namespace CarpetBG.Application.Interfaces.Services;

public interface IOrderPdfGenerator
{
    byte[] Generate(OrderPrintDto order);
}
