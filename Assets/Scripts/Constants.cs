using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public enum InputType
    {
        PIT_UP,
        PIT_DOWN,
        PIT_LEFT,
        PIT_RIGHT,

        PIT_INTERACT,
        PIT_ATTACK
    };

    public const string HumanTag = "Pawn";

    public const int PlayerSpeedX = 6;
    public const int PlayerSpeedZ = 6;

    public const KeyCode upKey = KeyCode.W;
    public const KeyCode downKey = KeyCode.S;
    public const KeyCode leftKey = KeyCode.A;
    public const KeyCode rightKey = KeyCode.D;

    public const KeyCode interactionKey = KeyCode.E;
    public const KeyCode attackKey = KeyCode.Space;
}
