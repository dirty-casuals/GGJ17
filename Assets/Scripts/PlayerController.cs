using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPawn
{
    private CharacterController controller;
    private Constants.PlayerState ePlayerState; //what is the player doing

    private GameObject goCarryPosition; //Position where the player carries stuff
    private GameObject goInteractPrompt;
    private GameObject goPlacementErrorPrompt;


    private GameObject goRageBar;
    private Sprite sprRageBar;
    private float fRageBarMaxXScale;
    private float fCurrentRageForScaling = 0;
    private float fLastRageForScaling = 0;

    private InteractionPoint pointLockedTo;

    public InteractionPoint InteractionLockedTo
    {
        get {
            return pointLockedTo;
        }
    }

    private GameObject currentInteractionGameObject; //Gameobject that the player is interacting with
    private InteractionPoint currentInteractionScript; //Handle to the interaction script

    private CustomerQueue goCurrentQueueHandle; //Queue handle if the interaction is with a queue


    public float fPlayerRage = Constants.PlayerStartingRage;
    public float fPlayerScore = 0;


    private float fItemPlacementErrorTimer = 0;

    private float fTaskTime;
    private float fRageTimer = 0.0f;

    private bool isMoving = false;
    private bool bInPlacementExclusionZone = false;

    public float speed
    {
        get { return isMoving ? Constants.PlayerSpeedZ : 0; }
    }

    public bool isHoldingItem
    {
        get; private set;
    }

    public event Action onPickupItem;
    public event Action onLeaveItem;
    public event Action onItemSwipe;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        goInteractPrompt = GameObject.FindGameObjectWithTag(Constants.PlayerInteractionPromptTag);
        goPlacementErrorPrompt = GameObject.FindGameObjectWithTag(Constants.PlayerPlacementErrorPromptTag);

        //Find and store the hand position
        foreach( Transform child in transform )
        {
            if( child.tag == Constants.PlayerCarryPos )
            {
                goCarryPosition = child.gameObject;
            }

            foreach (Transform childChildren in child)
            {
                if (childChildren.tag == Constants.CustomerRageBarTag)
                {
                    goRageBar = childChildren.gameObject;
                    sprRageBar = childChildren.GetComponent<Sprite>();
                    fRageBarMaxXScale = goRageBar.transform.localScale.x;

                    Vector3 vNewScale = goRageBar.transform.localScale;
                    vNewScale.x = 0;
                    goRageBar.transform.localScale = vNewScale;

                    break;
                }
            }
        }

        if(!goCarryPosition)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        if(!goInteractPrompt)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        if(!goPlacementErrorPrompt)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        goInteractPrompt.SetActive(false);
        goPlacementErrorPrompt.SetActive(false);
    }

    void Update()
    {
        UpdatePlayerMovement();

        switch( ePlayerState )
        {
            case Constants.PlayerState.PS_USING_TILL:
                {
                    ProcessCashierTask();
                    break;
                }
            case Constants.PlayerState.PS_CARRYING_FOODITEM:
                {
                    ProcessCarryTask();
                    break;
                }
        }

        fRageTimer += Time.deltaTime;

        if( fRageTimer > Constants.TickInterval )
        {
            fRageTimer = 0.0f;
            fPlayerRage += Constants.PlayerRageIncreasePerTick;
        }

        fLastRageForScaling = fCurrentRageForScaling;
        fCurrentRageForScaling = Mathf.Lerp(fCurrentRageForScaling, fPlayerRage, Constants.CustomerRageScaleFillRate * Time.deltaTime);

        if (fLastRageForScaling != fCurrentRageForScaling)
        {
            Vector3 vNewScale = goRageBar.transform.localScale;
            vNewScale.x = fRageBarMaxXScale * Constants.Normalise(fCurrentRageForScaling, 0, Constants.PlayerTillProgressToReach);
            goRageBar.transform.localScale = vNewScale;
        }

        if(fItemPlacementErrorTimer > 0)
        {
            fItemPlacementErrorTimer += Time.deltaTime;

            if(fItemPlacementErrorTimer > 0.5f)
            {
                fItemPlacementErrorTimer = 0;
                goPlacementErrorPrompt.SetActive(false);
            }
        }
    }

    //Rage
    public void Score_ProcessedCustomerItems( float fCustomerRage, float fCustomerWaitingTime )
    {
        //When putting items thru, the max score is decreased by customer rage and time waited
        fPlayerScore += Constants.PlayerScoreIncreaseForProcessingItems - (fCustomerRage * Constants.Normalise( fCustomerWaitingTime, 0, Constants.MaxCustomerWaitTime ));
        GameObject.FindGameObjectWithTag( Constants.UIControllerTag ).GetComponent<UIController>().UpdateScore( fPlayerScore );
    }
    public void Rage_ProcessedCustomerItems()
    {
        fPlayerRage += Constants.PlayerRageIncreaseProcessedItems;
    }
    public void Rage_KilledCustomer()
    {
        fPlayerRage -= Constants.PlayerRageDecreaseKilledCustomer;
    }

    //Tasking
    private void ProcessCashierTask()
    {
        if( currentInteractionGameObject )
        {
            //If we've moved away from the interaction till, we stop processing it
            if( Vector3.Distance( currentInteractionGameObject.transform.position, this.transform.position ) > currentInteractionScript.fInteractionRadius )
            {
                CleanupInteraction();
            }
            else
            {
                fTaskTime += Time.deltaTime;
                currentInteractionScript.AddProgress( Constants.PlayerTillIncreasePerTick * Time.deltaTime );

                //done with the current customer!
                if( currentInteractionScript.HasProgressCompleted() )
                {
                    Rage_ProcessedCustomerItems();
                    Score_ProcessedCustomerItems( goCurrentQueueHandle.GetCurrentCustomerRage(), goCurrentQueueHandle.GetCurrentCustomerTimeInQueue() );

                    currentInteractionScript.ResetProgress();
                    goCurrentQueueHandle.ReleaseCurrentCustomer();

                    if( goCurrentQueueHandle.GetCustomerCount() <= 1 )
                    {
                        CleanupInteraction();
                    }
                }
            }
        }
        else
        {
            CleanupInteraction();
        }
    }
    private void ProcessCarryTask()
    {
        //Set the item position to our carry position, update carry time
        currentInteractionGameObject.transform.position = goCarryPosition.transform.position;
        fTaskTime += Time.deltaTime;

        // Announce we're picking up
        if( !isHoldingItem )
        {
            isHoldingItem = true;

            if( onPickupItem != null )
                onPickupItem();
        }
    
        //Placing back down - timer to make sure we can't immediately place back down
        if( fTaskTime > 0.5f )
        {
            if( QueryPlayerInput( Constants.InputType.PIT_INTERACT, true ) )
            {
                if(currentInteractionScript.CanBePlaced() && !bInPlacementExclusionZone)
                {
                    ProcessPutItemOnShelf();
                }
                else
                {
                    fItemPlacementErrorTimer += Time.deltaTime;
                    goPlacementErrorPrompt.transform.position = this.transform.position + new Vector3(-0.37f,3,0);
                    goPlacementErrorPrompt.SetActive(true);
                }
            }
        }
    }
    private void ProcessPutItemOnShelf()
    {
        currentInteractionScript.PlaceItemBackOntoShelf();
        CleanupInteraction();

        // Announce we're picking up
        if( isHoldingItem )
        {
            isHoldingItem = false;

            if( onLeaveItem != null )
                onLeaveItem();
        }
    }
    private void SpawnNewFoodItemIntoHand(GameObject itemPrefab)
    {
        GameObject go = (GameObject)Instantiate(itemPrefab);
        go.layer = Constants.CarriedFoodItemLayer;
        currentInteractionGameObject = go;
        currentInteractionScript = go.GetComponent<InteractionPoint>();
        go.GetComponent<InteractionPoint>().DoStart();
    }

    //Getters
    public bool IsAbleToInteract()
    {
        return (!QueryPlayerInput( Constants.InputType.PIT_CAMERA_ZOOM_OUT ) && ePlayerState == Constants.PlayerState.PS_IDLE);
    }
    public bool IsServingCustomer()
    {
        return (ePlayerState == Constants.PlayerState.PS_USING_TILL);
    }
    public bool IsCarryingFoodItem()
    {
        return (ePlayerState == Constants.PlayerState.PS_CARRYING_FOODITEM);
    }

    //Player movement and input
    public bool QueryPlayerInput( Constants.InputType eType, bool bJustPressed = false )
    {
        switch( eType )
        {
            case Constants.InputType.PIT_UP: { return (!bJustPressed) ? Input.GetKey( Constants.upKey ) : Input.GetKeyDown( Constants.upKey ); }
            case Constants.InputType.PIT_DOWN: { return (!bJustPressed) ? Input.GetKey( Constants.downKey ) : Input.GetKeyDown( Constants.downKey ); }
            case Constants.InputType.PIT_LEFT: { return (!bJustPressed) ? Input.GetKey( Constants.leftKey ) : Input.GetKeyDown( Constants.leftKey ); }
            case Constants.InputType.PIT_RIGHT: { return (!bJustPressed) ? Input.GetKey( Constants.rightKey ) : Input.GetKeyDown( Constants.rightKey ); }
            case Constants.InputType.PIT_INTERACT: { return (!bJustPressed) ? Input.GetKey( Constants.interactionKey ) : Input.GetKeyDown( Constants.interactionKey ); }
            case Constants.InputType.PIT_ATTACK: { return (!bJustPressed) ? Input.GetKey( Constants.attackKey ) : Input.GetKeyDown( Constants.attackKey ); }

            case Constants.InputType.PIT_CAMERA_ZOOM_OUT: { return (!bJustPressed) ? Input.GetKey( Constants.zoomKey ) : Input.GetKeyDown( Constants.zoomKey ); }
        }

        return false;
    }
    private void UpdatePlayerMovement()
    {
        if( QueryPlayerInput( Constants.InputType.PIT_CAMERA_ZOOM_OUT ) )
        {
            isMoving = false;
            return;
        }

        Vector3 motion = Vector3.zero;

        if( QueryPlayerInput( Constants.InputType.PIT_UP ) )
            motion.z += 1;

        if( QueryPlayerInput( Constants.InputType.PIT_DOWN ) )
            motion.z -= 1;

        if( QueryPlayerInput( Constants.InputType.PIT_LEFT ) )
            motion.x -= 1;

        if( QueryPlayerInput( Constants.InputType.PIT_RIGHT ) )
            motion.x += 1;

        motion.Normalize();
        controller.SimpleMove( motion * Constants.PlayerSpeedZ );

        isMoving = (motion.magnitude > 0);

        if( isMoving )
        {
            Quaternion targetRotation = Quaternion.LookRotation( motion );
            transform.rotation =
                Quaternion.Slerp( transform.rotation,
                                  targetRotation,
                                  Time.deltaTime * Constants.PlayerRotationSpeed );
        }
    }

    //Interaction
    void CleanupInteraction()
    {
        currentInteractionGameObject.layer = Constants.DefaultItemLayer;

        if(currentInteractionScript)
        {
            currentInteractionScript.SetInUse( false );
            currentInteractionScript.ResetProgress( true );
        }

        ePlayerState = Constants.PlayerState.PS_IDLE;

        fTaskTime = 0;

        currentInteractionGameObject = null;
        goCurrentQueueHandle = null;
        currentInteractionScript = null;
    }
    void HandleInteraction( GameObject InteractGO )
    {
        //If we got in here and we're carrying food, we've interacted with a trash bin
        if(IsCarryingFoodItem())
        {
            if(currentInteractionGameObject)
            {
                currentInteractionScript = null;

                Destroy(currentInteractionGameObject);
                CleanupInteraction();
            }
            return;
        }

        currentInteractionScript = InteractGO.GetComponent<InteractionPoint>();

        if( currentInteractionScript )
        {
            currentInteractionGameObject = InteractGO;
            
            fTaskTime = 0;

            switch( currentInteractionScript.eInteractionType )
            {
                case Constants.InteractionPointType.IPT_FOOD_RESTOCK:
                    {
                        ePlayerState = Constants.PlayerState.PS_CARRYING_FOODITEM;
                        SpawnNewFoodItemIntoHand(currentInteractionScript.resupplyItemPrefabOverride);
                        currentInteractionScript.SetInUse( true );
                        break;
                    }
                case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                    {
                        ePlayerState = Constants.PlayerState.PS_CARRYING_FOODITEM;
                        currentInteractionGameObject.layer = Constants.CarriedFoodItemLayer;
                        currentInteractionScript.SetInUse( true );

                        break;
                    }
                case Constants.InteractionPointType.IPT_CASHIER_TILL:
                    {
                        goCurrentQueueHandle = currentInteractionGameObject.GetComponent<CustomerQueue>();

                        //Exit out if there isn't anyone in the queue
                        if( goCurrentQueueHandle.GetCustomerCount() == 0 )
                        {
                            CleanupInteraction();
                            return;
                        }

                        currentInteractionScript.SetInUse( true );
                        ePlayerState = Constants.PlayerState.PS_USING_TILL;

                        break;
                    }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //if idle and prompt is not up
        if(ePlayerState == Constants.PlayerState.PS_IDLE)
        {
            if(!goInteractPrompt.activeSelf)
            {
                //If we're in an interaction point
                if(other.GetComponent<InteractionPoint>() && other.GetComponent<InteractionPoint>().eInteractionType != Constants.InteractionPointType.IPT_FOOD_BIN)
                {
                    if(other.GetComponent<InteractionPoint>().CustomInteraction == new Vector3(-1,-1,-1))
                    {
                        goInteractPrompt.transform.position = other.transform.position + new Vector3(0,1,0);
                    }
                    else
                    {
                        goInteractPrompt.transform.position = other.GetComponent<InteractionPoint>().CustomInteraction;
                    }
                    goInteractPrompt.SetActive(true);

                    pointLockedTo = other.GetComponent<InteractionPoint>();
                }
            }
        }
        else
        {
            if(goInteractPrompt.activeSelf)
            {
                //If we're in an interaction point
                if(other.GetComponent<InteractionPoint>())
                {
                    goInteractPrompt.SetActive(false);
                    pointLockedTo = null;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Constants.PlayerPlacementExclusionZoneTag)
        {
            bInPlacementExclusionZone = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == Constants.PlayerPlacementExclusionZoneTag)
        {
            bInPlacementExclusionZone = false;
        }

        if(goInteractPrompt.activeSelf)
        {
            //If we're in an interaction point
            if(other.GetComponent<InteractionPoint>())
            {
                goInteractPrompt.SetActive(false);
            }
        }
    }
}
