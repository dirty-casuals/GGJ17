using System;

public interface IPawn
{
    event Action onPickupItem;
    event Action onLeaveItem;
    event Action onItemSwipe;

    float speed { get; }
    bool isHoldingItem { get; }
}
