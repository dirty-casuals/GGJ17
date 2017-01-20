using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_UP))
        {
            this.transform.position += new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_DOWN))
        {
            this.transform.position -= new Vector3(0, 0, Constants.PlayerSpeedZ) * Time.deltaTime;
        }

        if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_LEFT))
        {
            this.transform.position -= new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
        }
        else if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_RIGHT))
        {
            this.transform.position += new Vector3(Constants.PlayerSpeedX, 0, 0) * Time.deltaTime;
        }
	}

    void HandleInteraction(GameObject InteractGO)
    {
        InteractionPoint interactionHandle = InteractGO.GetComponent<InteractionPoint>();

        if(interactionHandle)
        {
            switch (interactionHandle.eInteractionType)
            {
                case InteractionPoint.InteractionPointType.IPT_FOOD_PRODUCT:
                {
                    TaskPickupFoodItem(InteractGO);
                    break;
                }
                case InteractionPoint.InteractionPointType.IPT_CASHIER_TILL:
                {
                    TaskProcessCustomerQueue();
                    break;
                }
            }
        }
    }

    void TaskPickupFoodItem(GameObject go)
    {

    }

    void TaskProcessCustomerQueue()
    {

    }
}
