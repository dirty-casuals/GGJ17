using System;

public interface IPawn
{
    event Action onPickupItem;
    event Action onLeaveItem;
    event Action onHit;

    float speed { get; }
    bool isHoldingItem { get; }
    bool isSwippingItems { get; }
}
