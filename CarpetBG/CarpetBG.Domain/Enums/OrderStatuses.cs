namespace CarpetBG.Domain.Enums;

public enum OrderStatuses
{
    New = 0,
    PendingPickup = 1,
    PickupComplete = 2,
    WashinginProgress = 3,
    WashingComplete = 4,
    /// <summary>
    /// Represents a delivery method where the item is picke up from teh store.
    /// </summary>
    /// <remarks>This delivery method typically involves direct handoff to the recipient, ensuring secure and
    /// personal delivery.</remarks>
    PersonalDelivery = 5,
    PendingDelivery = 6,
    DeliveryComplete = 7,

    Cancelled = 99,
    Completed = 100,
}
