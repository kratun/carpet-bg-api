using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Common;

public class StatusFilter
{
    public OrderStatuses Status { get; set; }
    public DateTime? Date { get; set; }
}
