using CarpetBG.Application.DTOs.Orders;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CarpetBG.Application.Documents;

public class OrderPdfDocument(OrderPrintDto model) : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(25);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ---------------- HEADER ----------------
    void ComposeHeader(IContainer container)
    {
        container.PaddingBottom(4).Row(row =>
        {
            // LEFT
            row.RelativeItem().AlignLeft().Column(col =>
            {
                col.Item().Text("CarpetBG").Bold().FontSize(12);
            });

            // CENTER (real page center)
            row.RelativeItem().AlignBottom().AlignCenter().Column(col =>
            {
                col.Item().AlignCenter().Text($"Поръчка №: {model.OrderNumber}/{model.CreatedAt}")
                    .Bold()
                    .FontSize(12);
            });

            // RIGHT
            row.RelativeItem().AlignBottom().AlignRight().Column(col =>
            {
                col.Item().AlignRight().Text("");
            });
        });
    }

    // ---------------- CONTENT ----------------
    void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(10);

            column.Item().Element(ComposeCustomerBlock);
            column.Item().Element(ComposeItemsTable);
            column.Item().Element(ComposeTerms);
            column.Item().Element(ComposeSignatures);
        });
    }

    void ComposeCustomerBlock(IContainer container)
    {
        container.Border(1).Padding(10).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"Клиент: {model.CustomerFullName}").Bold();
                col.Item().Text($"Телефон: {model.PhoneNumber}");

                col.Item().Text($"Получена от клиента: {model.PickupDate}");
                col.Item().Text($"Издадена на клиента: {model.DeliveryDate}");
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text(text =>
                {
                    text.Span("Адрес за вземане: ").Bold();
                    text.Span($"{model.PickupAddress}");
                });
                col.Item().Text(text =>
                {
                    text.Span("Адрес за доставка: ").Bold();
                    text.Span(model.DeliveryAddress);
                });
            });
        });
    }

    // ---------------- ITEMS TABLE ----------------
    void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);
                columns.RelativeColumn(4);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("№").Bold();
                header.Cell().Element(CellStyle).AlignCenter().Text("Артикул").Bold();
                header.Cell().Element(CellStyle).AlignCenter().Text("Цена").Bold();
                header.Cell().Element(CellStyle).AlignCenter().Text("Кол.").Bold();
                header.Cell().Element(CellStyle).AlignCenter().Text("Сума").Bold();
            });

            int index = 1;

            foreach (var item in model.OrderItems)
            {
                if (item.IsPlaceholder)
                {
                    table.Cell().Element(CellStyle).Height(22);
                    table.Cell().Element(CellStyle);
                    table.Cell().Element(CellStyle);
                    table.Cell().Element(CellStyle);
                    table.Cell().Element(CellStyle);
                    continue;
                }
                var measurement = !string.IsNullOrEmpty(item.Width) && !string.IsNullOrEmpty(item.Width)
                ? $"{item.Width}x{item.Height}"
                : string.Empty;

                table.Cell().Element(CellStyle).Text($"{index++}.");
                table.Cell().Element(CellStyle).Row(r =>
                {
                    // left side (product)
                    r.RelativeItem().AlignLeft().Text(item.ProductName);

                    // right side (measurement)
                    r.ConstantItem(60)
                        .AlignRight()
                        .Text(item.Measurment);
                });
                table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice);
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
                table.Cell().Element(CellStyle).AlignRight().Text(item.Amount);
            }

            table.Footer(footer =>
            {
                footer.Cell().AlignCenter().Text("").Bold();
                footer.Cell().AlignCenter().Text("").Bold();
                footer.Cell().Element(CellStyle).AlignRight().Text("Общо:").Bold();
                footer.Cell().Element(CellStyle).AlignRight().Text(model.TotalQuantity).Bold();
                footer.Cell().Element(CellStyle).AlignRight().Text(model.TotalAmount).Bold();
            });
        });
    }

    // ---------------- TERMS ----------------
    void ComposeTerms(IContainer container)
    {
        container.PaddingTop(-16).Column(col =>
        {
            col.Item().Text("Общи условия").Bold().FontSize(12);

            col.Spacing(4);

            TermsItem(col.Item(), "1.", "Заявки за пране на килими се приемат на телефон 02/99 505 99 и 0879/ 801 666 или на място в офиса на Carpet.BG.");

            TermsItem(col.Item(), "2.", "Килимите Ви се приемат от служителите на Carpet.BG предварително почистени (с прахосмукачка) и навити на руло.");

            TermsItem(col.Item(), "3.", "Служителите на Carpet.BG нямат право да влизат в домовете на клиентите, както и да извършват други дейности несвързани с взимането и транспортирането на килими.");

            TermsItem(col.Item(), "4.", "Carpet.BG не носи отговорност за килими: скъсани, изгорени, изгнили, с разплетен оверлог, проядени от молци, с петна от восък, вино, кръв, маркер, боя, оцветени (с нестабилни цветове, които пускат) или избелели цветове и всички други замърсявания и увреждания, причинени от нетипична и неправилна експлоатация.");

            TermsItem(col.Item(), "5.", "Carpet.BG извършва пране на килими, а не химическо почистване на килими.");

            TermsItem(col.Item(), "6.", "Цените на Carpet.BG се образуват спрямо размера на килима или избраната услугата и са упоменати в раздел \"Цени\".");

            TermsItem(col.Item(), "7.", "В рамките на седем дни или друг уговорен срок от взимане на поръчката, Carpet.BG и КЛИЕНТЪТ се задължават да осигурят връщането й.");

            TermsItem(col.Item(), "8.", "Carpet.BG доставя изпраните и изсушени килими до адреса на клиента в уговорен ден и час, в случай на неосигурен приемащ килима, всяко следващо транспортиране се таксува спрямо цената обявена в раздел \"Цени\".");

            TermsItem(col.Item(), "9.", "Рекламации се приемат съгласно чл. 126 ал 1 от ЗЗП.");

            TermsItem(col.Item(), "10.", "С поръчка на услугата КЛИЕНТЪТ приема общите условия на Carpet.BG.");

            TermsItem(col.Item(), "11.", "Фиксинг на БНБ към 31.12.2025 г. 1 евро = 1.95583 лв.");
        });
    }

    // ---------------- SIGNATURES ----------------
    void ComposeSignatures(IContainer container)
    {
        container.PaddingTop(30).Row(row =>
        {
            // Left: Клиент
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Клиент:").FontSize(10).Bold();
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Darken1)
                    .Height(1); // thin underline
            });

            // Spacer
            row.ConstantItem(50); // space between signatures

            // Right: Доставчик
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Доставчик:").FontSize(10).Bold();
                col.Item()
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Darken1)
                    .Height(1); // thin underline
            });
        });
    }

    // ---------------- FOOTER ----------------
    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Страница ");
            text.CurrentPageNumber();
            text.Span(" / ");
            text.TotalPages();
        });
    }

    void TermsItem(IContainer container, string number, string text)
    {
        container.Row(row =>
        {
            // number column (fixed width)
            row.ConstantItem(24).AlignLeft().Text(number);

            // text column (fills remaining width)
            row.RelativeItem().Text(text);
        });
    }

    IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .PaddingVertical(6)
            .PaddingHorizontal(4)
            .AlignMiddle();
    }
}
