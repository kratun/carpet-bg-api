using CarpetBG.Application.Services;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IOrderDocumentService
{
    Task<Result<OrderDocumentService.PdfData>> GenerateOrderPdfAsync(Guid orderId);
}
