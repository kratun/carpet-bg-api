namespace CarpetBG.Domain.Enums;

public enum OrderStatuses
{
    New = 0,
    PrePickupSetup = 1,
    PendingPickup = 2,
    PickupComplete = 3,
    WashingInProgress = 4,
    WashingComplete = 5,
    PreDeliverySetup = 6,
    PendingDelivery = 7,
    /// <summary>
    /// Represents a delivery method where the item is picke up from teh store.
    /// </summary>
    /// <remarks>This delivery method typically involves direct handoff to the recipient, ensuring secure and
    /// personal delivery.</remarks>
    PersonalDelivery = 8,
    DeliveryComplete = 9,

    Cancelled = 99,
    Completed = 100,
}
