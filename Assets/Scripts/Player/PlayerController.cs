using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Constants.PlayerState ePlayerState;
    private GameObject goCurrentInteractionGO, goPlayerCarry;
    private CustomerQueue goCurrentQueueHandle;
    private InteractionPoint currentInteractionHandle;

    //till processing
    private float fTimeAtTill;
    private float fTimeCarryingItem;
    

	// Use this for initialization
	void Start ()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == Constants.PlayerCarryPos)
            {
                goPlayerCarry = child.gameObject;
            }
        }
	}

    public bool IsAbleToInteract()
    {
        return (ePlayerState == Constants.PlayerState.PS_IDLE);
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePlayerMovement();

        switch (ePlayerState)
        {
            case Constants.PlayerState.PS_USING_TILL:
            {
                TaskProcessCustomerQueue();
                break;
            }
            case Constants.PlayerState.PS_CARRYING_FOODITEM:
            {
                TaskPickupFoodItem();
                break;
            }
        }
	}

    void HandleInteraction(GameObject InteractGO)
    {
        currentInteractionHandle = InteractGO.GetComponent<InteractionPoint>();

        if(currentInteractionHandle)
        {
            goCurrentInteractionGO = InteractGO;

            switch (currentInteractionHandle.eInteractionType)
            {
                case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                {
                    ePlayerState = Constants.PlayerState.PS_CARRYING_FOODITEM;
                    fTimeCarryingItem = 0;
                    currentInteractionHandle.SetInUse(true);
                    goCurrentInteractionGO.layer = Constants.CarriedFoodItemLayer;
                    //InteractGO.GetComponent<HingeJoint>().connectedBody = goPlayerCarry.GetComponent<Rigidbody>();
                    
                    break;
                }
                case Constants.InteractionPointType.IPT_CASHIER_TILL:
                {
                    goCurrentQueueHandle = goCurrentInteractionGO.GetComponent<CustomerQueue>();
                    if(goCurrentQueueHandle.GetCustomerCount() == 0)
                    {
                        CleanupInteraction();
                        return;
                    }

                    ePlayerState = Constants.PlayerState.PS_USING_TILL;
                    
                    currentInteractionHandle.SetInUse(true);
                    fTimeAtTill = 0;
                    break;
                }
            }
        }
    }

    void TaskPickupFoodItem()
    {
        goCurrentInteractionGO.transform.position = goPlayerCarry.transform.position;
        fTimeCarryingItem += Time.deltaTime;

        if(currentInteractionHandle.CanBePlaced() && fTimeCarryingItem > 0.5f) //just make sure it isn't instantly placed back down
        {
            if (PlayerInput.QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
            {
                AlignFoodItemWithShelf();
                CleanupInteraction();
            }
        }
    }

    void AlignFoodItemWithShelf()
    {
        //Match it to the shelf height
        if(currentInteractionHandle.goShelfBeingPlacedOnTo != null)
        {
            Vector3 vNewPos = goCurrentInteractionGO.transform.position;
                    
            //Height
            foreach (Transform child in currentInteractionHandle.goShelfBeingPlacedOnTo.transform)
            {
                if (child.tag == Constants.ShelfHeightTag)
                {
                    vNewPos.y = child.gameObject.transform.position.y;
                    break;
                }
            }

            Vector3 shelfPos = currentInteractionHandle.goShelfBeingPlacedOnTo.transform.position;
            Vector3 shelfScale = currentInteractionHandle.goShelfBeingPlacedOnTo.transform.localScale;

            //top side of shelf
            if(vNewPos.z > shelfPos.z)
            {
                vNewPos.z = shelfPos.z + shelfScale.z / 2;
            }
            else
            {
                vNewPos.z = shelfPos.z - shelfScale.z / 2;
            }

            //make sure the thing is on the shelf - more to the right
            if(vNewPos.x > shelfPos.x + (shelfScale.x / 2))
            {
                vNewPos.x = (shelfPos.x + (shelfScale.x / 2)) - goCurrentInteractionGO.transform.localScale.x;
            }
            else if(vNewPos.x < shelfPos.x - (shelfScale.x / 2))
            {
                vNewPos.x = (shelfPos.x - (shelfScale.x / 2)) + goCurrentInteractionGO.transform.localScale.x;
            }

           

            goCurrentInteractionGO.transform.position = vNewPos;
        }
    }

    void TaskProcessCustomerQueue()
    {
        if (goCurrentInteractionGO)
        {
            //If we've moved away from the interaction till, we stop processing it
            if (Vector3.Distance(goCurrentInteractionGO.transform.position, this.transform.position) > currentInteractionHandle.fInteractionRadius)
            {
                CleanupInteraction();
            }
            else
            {
                fTimeAtTill += Time.deltaTime;
                currentInteractionHandle.AddProgress(Constants.PlayerTillIncreasePerTick * Time.deltaTime);

                //done with the current customer!
                if (currentInteractionHandle.HasProgressCompleted())
                {
                    currentInteractionHandle.ResetProgress();
                    this.GetComponent<PlayerRage>().Rage_ProcessedCustomerItems();
                    goCurrentQueueHandle.ReleaseCurrentCustomer();

                    //this.GetComponent<PlayerRage>().Score_ProcessedCustomerItems();


                    if(goCurrentQueueHandle.GetCustomerCount() == 0)
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


    void CleanupInteraction()
    {
        goCurrentInteractionGO.layer = Constants.DefaultItemLayer;

        currentInteractionHandle.SetInUse(false);
        ePlayerState = Constants.PlayerState.PS_IDLE;

        goCurrentInteractionGO = null;
        goCurrentQueueHandle = null;
        currentInteractionHandle = null;
    }




    void UpdatePlayerMovement()
    {
        if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_UP))
        {
            this.transform.position += new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_DOWN))
        {
            this.transform.position -= new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*180), Constants.PlayerRotationSpeed * Time.deltaTime);
        }

        if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_LEFT))
        {
            this.transform.position -= new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.down*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_RIGHT))
        {
            this.transform.position += new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(Vector3.up*90), Constants.PlayerRotationSpeed * Time.deltaTime);
        }
    }
}
