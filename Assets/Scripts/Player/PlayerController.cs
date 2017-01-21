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

    void HandleInteraction(GameObject InteractGO)
    {
        InteractionPoint interactionHandle = InteractGO.GetComponent<InteractionPoint>();

        if(interactionHandle)
        {
            switch (interactionHandle.eInteractionType)
            {
                case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                {
                    TaskPickupFoodItem(InteractGO);
                    break;
                }
                case Constants.InteractionPointType.IPT_CASHIER_TILL:
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
