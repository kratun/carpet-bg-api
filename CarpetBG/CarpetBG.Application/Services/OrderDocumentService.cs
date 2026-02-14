using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class OrderDocumentService(
    IOrderRepository orderRepository,
    IOrderFactory orderFactory,
    IOrderPdfGenerator pdfGenerator) : IOrderDocumentService
{
    public async Task<Result<PdfData>> GenerateOrderPdfAsync(Guid orderId)
    {
        try
        {
            var order = await orderRepository.GetByIdWithAllRelatedDataAsync(orderId);

            if (order == null)
            {
                return Result<PdfData>.Failure("Order not found");
            }

            var printOrder = orderFactory.MapToPrintModel(order, 8);


            // Offload CPU heavy PDF generation
            var pdfBytes = await Task.Run(() => pdfGenerator.Generate(printOrder));

            if (pdfBytes.Length == 0)
                return Result<PdfData>.Failure("Failed to generate document");

            var fileName = $"order-{printOrder.OrderNumber}-{printOrder.CreatedAt}";
            var pdfData = new PdfData(pdfBytes, fileName);
            return Result<PdfData>.Success(pdfData);
        }
        catch (Exception)
        {
            // Do NOT leak internal errors
            return Result<PdfData>.Failure("Unable to generate order document");
        }
    }

    public sealed record PdfData(byte[] PdfBytes, string PdfFileName)
    {
    }
}
