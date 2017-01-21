using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public const float TickInterval = 1.0f;

    public enum PlayerState
    {
        PS_IDLE,
        PS_USING_TILL,
        PS_CARRYING_FOODITEM
    };

    public enum InputType
    {
        PIT_UP,
        PIT_DOWN,
        PIT_LEFT,
        PIT_RIGHT,

        PIT_INTERACT,
        PIT_ATTACK,

        PIT_CAMERA_ZOOM_OUT
    };

    public enum InteractionPointType
    {
        IPT_INVALID,

        IPT_CASHIER_TILL,
        IPT_FOOD_PRODUCT,
        IPT_FREEZER
    };

    public const int DefaultItemLayer = 0;
    public const int CarriedFoodItemLayer = 8;

    public const string PlaceableShelfTag = "FoodShelf";
    public const string ShelfHeightTag = "ShelfHeightGO";

    public const string CustomInteractionUIPointTag = "CustomInteractionButtonPlacement";

    public const string PlayerTag = "Player";
    public const string PlayerKillAreaTag = "PlayerKillArea";
    public const string CustomerTag = "Pawn";
    public const string PlayerCamPosTag = "PlayerCamPos";
    public const string PlayerCarryPos = "PlayerHandPos";

    public const string CustomerRageBarTag = "RageBar";

    public const string CustomerQueueTag = "CustomerQueue";


    public const string IPProgressBarTag = "ProgressBar";
    public const string IPProgressBarBGTag = "ProgressBarBG";
    public const string IPProgressTextTag = "ProgressText";

    public const string PlayerInteractionFunction = "HandleInteraction";

    public const string ScoreUITag = "ScoreUI";
    public const string UIControllerTag = "UIController";

    public const string PlayerInteractionPromptTag = "InteractionPrompt";
    public const string PlayerKillPromptTag = "KillPrompt";

    public const float IPProgressTextBlinkTime = 0.75f;


    public const int PlayerSpeedX = 6;
    public const int PlayerSpeedZ = 6;

    public const int CameraSpeedX = 24;
    public const int CameraSpeedZ = 24;

    public const float PlayerCamFollowSpeed = 2.0f;
    public const float PlayerRotationSpeed = 5.0f;


    public const float PlayerCameraOffsetY = 10.0f;
    public const float PlayerCameraOffsetZ = -3.5f;

    public const float PlayerCameraZoomOutOffsetY = 30.0f;


    public const float AvgQueueRageForGameFail = 75.0f;

    public const int PlayerScoreIncreaseForProcessingItems = 100;
    public const float MaxCustomerWaitTime = 30.0f;


    public const float PlayerStartingRage = 15.0f;
    public const float PlayerRageIncreasePerTick = 0.5f;
    public const float PlayerRageIncreaseProcessedItems = 7.5f;
    public const float PlayerRageDecreaseKilledCustomer = 15.0f;

    public const float CustomerRageIncreaseOutOfStockItem = 20.0f;
    public const float CustomerRageInQueueIncreasePerTick = AvgQueueRageForGameFail / MaxCustomerWaitTime;

    public const float CustomerRageScaleFillRate = 2.5f;


    public const float PlayerTillIncreasePerTick = 10.0f;
    public const float PlayerTillProgressToReach = 100.0f;
    

    public const KeyCode upKey = KeyCode.W;
    public const KeyCode downKey = KeyCode.S;
    public const KeyCode leftKey = KeyCode.A;
    public const KeyCode rightKey = KeyCode.D;

    public const KeyCode interactionKey = KeyCode.E;
    public const KeyCode attackKey = KeyCode.Q;
    public const KeyCode zoomKey = KeyCode.Space;

    public static float Normalise(float val, float min, float max)
    {
        return ( (val - min) / (max - min) );
    }
}
