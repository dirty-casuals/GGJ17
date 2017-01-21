using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Constants.PlayerState ePlayerState; //what is the player doing

    private GameObject goCarryPosition; //Position where the player carries stuff

    private GameObject currentInteractionGameObject; //Gameobject that the player is interacting with
    private InteractionPoint currentInteractionScript; //Handle to the interaction script

    private CustomerQueue goCurrentQueueHandle; //Queue handle if the interaction is with a queue
    

    public float fPlayerRage = Constants.PlayerStartingRage;
    public float fPlayerScore = 0;


    private float fTaskTime;
    private float fRageTimer = 0.0f;

	
	void Start ()
    {
        //Find and store the hand position
		foreach (Transform child in transform)
        {
            if (child.tag == Constants.PlayerCarryPos)
            {
                goCarryPosition = child.gameObject;
            }
        }
	}
	
	void Update ()
    {
		UpdatePlayerMovement();

        switch (ePlayerState)
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

        if(fRageTimer > Constants.TickInterval)
        {
            fRageTimer = 0.0f;
            fPlayerRage += Constants.PlayerRageIncreasePerTick;
        }
	}

    //Rage
    public void Score_ProcessedCustomerItems(float fCustomerRage, float fCustomerWaitingTime)
    {
        //When putting items thru, the max score is decreased by customer rage and time waited
        fPlayerScore += Constants.PlayerScoreIncreaseForProcessingItems - (fCustomerRage * (1 - Constants.Normalise(fCustomerWaitingTime, 0, Constants.MaxCustomerWaitTime) ));
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
        if (currentInteractionGameObject)
        {
            //If we've moved away from the interaction till, we stop processing it
            if (Vector3.Distance(currentInteractionGameObject.transform.position, this.transform.position) > currentInteractionScript.fInteractionRadius)
            {
                CleanupInteraction();
            }
            else
            {
                fTaskTime += Time.deltaTime;
                currentInteractionScript.AddProgress(Constants.PlayerTillIncreasePerTick * Time.deltaTime);

                //done with the current customer!
                if (currentInteractionScript.HasProgressCompleted())
                {
                    currentInteractionScript.ResetProgress();
                    goCurrentQueueHandle.ReleaseCurrentCustomer();

                    Rage_ProcessedCustomerItems();
                    Score_ProcessedCustomerItems(goCurrentQueueHandle.GetCurrentCustomerRage(), goCurrentQueueHandle.GetCurrentCustomerTimeInQueue());
                    
                    if(goCurrentQueueHandle.GetCustomerCount() <= 1)
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

        //Placing back down - timer to make sure we can't immediately place back down
        if(currentInteractionScript.CanBePlaced() && fTaskTime > 0.5f)
        {
            if (QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
            {
                ProcessPutItemOnShelf();
                
            }
        }
    }
    private void ProcessPutItemOnShelf()
    {
        currentInteractionScript.PlaceItemBackOntoShelf();
        CleanupInteraction();
    }

    //Getters
    public bool IsAbleToInteract()
    {
        return (ePlayerState == Constants.PlayerState.PS_IDLE);
    }


    //Player movement and input
    public bool QueryPlayerInput(Constants.InputType eType, bool bJustPressed = false)
    {
        switch(eType)
        {
            case Constants.InputType.PIT_UP: {  return (!bJustPressed) ? Input.GetKey(Constants.upKey) : Input.GetKeyDown(Constants.upKey); }
            case Constants.InputType.PIT_DOWN: {  return (!bJustPressed) ? Input.GetKey(Constants.downKey) : Input.GetKeyDown(Constants.downKey); }
            case Constants.InputType.PIT_LEFT: {  return (!bJustPressed) ? Input.GetKey(Constants.leftKey) : Input.GetKeyDown(Constants.leftKey); }
            case Constants.InputType.PIT_RIGHT: {  return (!bJustPressed) ? Input.GetKey(Constants.rightKey) : Input.GetKeyDown(Constants.rightKey); }

            case Constants.InputType.PIT_INTERACT: {  return (!bJustPressed) ? Input.GetKey(Constants.interactionKey) : Input.GetKeyDown(Constants.interactionKey); }
            case Constants.InputType.PIT_ATTACK: {  return (!bJustPressed) ? Input.GetKey(Constants.attackKey) : Input.GetKeyDown(Constants.attackKey); }
        }

        return false;
    }
    private void UpdatePlayerMovement()
    {
        if(QueryPlayerInput(Constants.InputType.PIT_UP))
        {
            this.transform.position += new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(QueryPlayerInput(Constants.InputType.PIT_DOWN))
        {
            this.transform.position -= new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*180), Constants.PlayerRotationSpeed * Time.deltaTime);
        }

        if(QueryPlayerInput(Constants.InputType.PIT_LEFT))
        {
            this.transform.position -= new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.down*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(QueryPlayerInput(Constants.InputType.PIT_RIGHT))
        {
            this.transform.position += new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
    }

    //Interaction
    void CleanupInteraction()
    {
        currentInteractionGameObject.layer = Constants.DefaultItemLayer;

        currentInteractionScript.SetInUse(false);
        ePlayerState = Constants.PlayerState.PS_IDLE;

        fTaskTime = 0;

        currentInteractionGameObject = null;
        goCurrentQueueHandle = null;
        currentInteractionScript = null;
    }
    void HandleInteraction(GameObject InteractGO)
    {
        currentInteractionScript = InteractGO.GetComponent<InteractionPoint>();

        if(currentInteractionScript)
        {
            currentInteractionGameObject = InteractGO;
            currentInteractionScript.SetInUse(true);
            fTaskTime = 0;

            switch (currentInteractionScript.eInteractionType)
            {
                case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                {
                    ePlayerState = Constants.PlayerState.PS_CARRYING_FOODITEM;
                    currentInteractionGameObject.layer = Constants.CarriedFoodItemLayer;

                    break;
                }
                case Constants.InteractionPointType.IPT_CASHIER_TILL:
                {
                    goCurrentQueueHandle = currentInteractionGameObject.GetComponent<CustomerQueue>();
                    
                    //Exit out if there isn't anyone in the queue
                    if(goCurrentQueueHandle.GetCustomerCount() == 0)
                    {
                        CleanupInteraction();
                        return;
                    }

                    ePlayerState = Constants.PlayerState.PS_USING_TILL;
                    
                    break;
                }
            }
        }
    }
}
