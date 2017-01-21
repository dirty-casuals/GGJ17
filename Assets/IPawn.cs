using System;

public interface IPawn
{
    event Action onPickup;
    event Action onItemSwipe;

    float speed { get; }
}
