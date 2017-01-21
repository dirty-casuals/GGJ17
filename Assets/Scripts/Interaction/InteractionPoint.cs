using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    

    public Constants.InteractionPointType eInteractionType;
    public float fInteractionRadius = 0.5f;

    private SphereCollider sCollision;

	// Use this for initialization
	void Start () {
        sCollision = gameObject.AddComponent<SphereCollider>();

        sCollision.radius = fInteractionRadius;
        sCollision.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerStay(Collider other)
    {
        if(other.tag == Constants.PlayerTag)
        {
            if(PlayerInput.QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
            {
                ProcessInteraction(other.gameObject);
            }
        }
    }

    private void ProcessInteraction(GameObject other)
    {
        Debug.Log("Player Interacted with me");
        other.SendMessage("HandleInteraction", this.gameObject);
    }


    void OnDrawGizmosSelected()
    {
        switch(eInteractionType)
        {
            case Constants.InteractionPointType.IPT_INVALID:
                Gizmos.color = Color.red;
            break;

            case Constants.InteractionPointType.IPT_CASHIER_TILL:
                Gizmos.color = new Color(255, 128, 0);
            break;

            case Constants.InteractionPointType.IPT_FOOD_PRODUCT:
                Gizmos.color = Color.green;
            break;

            case Constants.InteractionPointType.IPT_FREEZER:
                Gizmos.color = Color.blue;
            break;
        }
        
        Gizmos.DrawWireSphere(transform.position, fInteractionRadius);
    }
}
