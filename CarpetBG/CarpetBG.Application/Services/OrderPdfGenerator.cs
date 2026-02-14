using CarpetBG.Application.Documents;
using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Services;

using QuestPDF.Fluent;

namespace CarpetBG.Application.Services;

public class OrderPdfGenerator : IOrderPdfGenerator
{
    public byte[] Generate(OrderPrintDto order)
    {
        var document = new OrderPdfDocument(order);
        return document.GeneratePdf();
    }
}
