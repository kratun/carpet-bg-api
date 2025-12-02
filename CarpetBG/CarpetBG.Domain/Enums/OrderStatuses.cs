namespace CarpetBG.Domain.Enums;

public enum OrderStatuses
{
    New = 0,
    PendingPickup = 1,
    PickupComplete = 2,
    WashinginProgress = 3,
    WashingComplete = 4,
    PersonalDelivery = 5,
    PendingDelivery = 6,
    DeliveryComplete = 7,

    Cancelled = 99,
    Completed = 100,
}
