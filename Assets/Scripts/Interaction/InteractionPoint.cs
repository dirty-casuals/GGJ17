using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    //Progress stuff - not all interaction points have this
    private float fCurrentProgressForScaling;
    private float fLastProgressForScaling;
    private GameObject goProgressBar, goProgressBarBG, goProcessingText;
    private float fProgress;
    private float fProgressMax;
    private float fProgressTextBlinkTimer = Constants.IPProgressTextBlinkTime; //so it instantly comes on


    private bool bInUse;

    private bool bCanBePlaced = false;
    public GameObject goShelfBeingPlacedOnTo;


    public Constants.InteractionPointType eInteractionType;
    public float fInteractionRadius = 0.5f;

    private SphereCollider sCollision;

	void Start ()
    {
        sCollision = gameObject.AddComponent<SphereCollider>();

        sCollision.radius = fInteractionRadius;
        sCollision.isTrigger = true;

        //get the ui elements
        foreach (Transform child in transform)
        {
            if (child.tag == Constants.IPProgressBarTag)
            {
                goProgressBar = child.gameObject;
                fProgressMax = goProgressBar.transform.localScale.x;

                Vector3 vNewScale = goProgressBar.transform.localScale;
                vNewScale.x = 0;
                goProgressBar.transform.localScale = vNewScale;
            }

            if (child.tag == Constants.IPProgressBarBGTag)
            {
                goProgressBarBG = child.gameObject;
            }

            if (child.tag == Constants.IPProgressTextTag)
            {
                goProcessingText = child.gameObject;
                goProcessingText.SetActive(false);
            }
        }
	}


    //We can use the UI as a measure instead so it looks like the bar has fully filled up
    public bool HasProgressCompleted(bool bUseUI = true)
    {
        return bUseUI ? fCurrentProgressForScaling >= Constants.PlayerTillProgressToReach : fProgress >= Constants.PlayerTillProgressToReach;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (bInUse && eInteractionType == Constants.InteractionPointType.IPT_CASHIER_TILL)
        {
            fLastProgressForScaling = fCurrentProgressForScaling;
            fCurrentProgressForScaling = Mathf.Lerp(fCurrentProgressForScaling, fProgress, Constants.CustomerRageScaleFillRate * Time.deltaTime);

            //Update the UI
            if (fCurrentProgressForScaling != fLastProgressForScaling)
            {
                Vector3 vNewScale = goProgressBar.transform.localScale;
                vNewScale.x = fProgressMax * Constants.Normalise(fCurrentProgressForScaling, 0, Constants.PlayerTillProgressToReach);
                goProgressBar.transform.localScale = vNewScale;
            }

            fProgressTextBlinkTimer += Time.deltaTime;

            if (fProgressTextBlinkTimer >= Constants.IPProgressTextBlinkTime)
            {
                fProgressTextBlinkTimer = 0;
                goProcessingText.SetActive(!goProcessingText.activeSelf);
            }
        }
        else if(bInUse && eInteractionType == Constants.InteractionPointType.IPT_FOOD_PRODUCT)
        {
            
        }
	}



    void OnTriggerStay(Collider other)
    {
        if (!bInUse)
        {
            if (other.tag == Constants.PlayerTag)
            {
                PlayerController controllerHandle = other.GetComponent<PlayerController>();
                if(controllerHandle && controllerHandle.IsAbleToInteract())
                {
                    if (controllerHandle.QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
                    {
                        other.SendMessage(Constants.PlayerInteractionFunction, this.gameObject);
                    }
                }
            }
        }
        else if(eInteractionType == Constants.InteractionPointType.IPT_FOOD_PRODUCT)
        {
            bCanBePlaced = (other.tag == Constants.PlaceableShelfTag);

            if(bCanBePlaced)
            {
                goShelfBeingPlacedOnTo = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(bInUse)
        {
            bCanBePlaced = false;
        }
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
    public void SetInUse(bool inUse)
    {
        bInUse = inUse;
    }
    public bool InUse()
    {
        return bInUse;
    }
    public float GetProgress()
    {
        return fProgress;
    }
    public bool CanBePlaced()
    {
        return bCanBePlaced;
    }
    public void AddProgress(float prog)
    {
        fProgress += prog;
    }

    public void ResetProgress(bool bOnlyClearProcessingText = false)
    {

        fProgressTextBlinkTimer = Constants.IPProgressTextBlinkTime; //so it instantly comes on

        if(goProcessingText)
            goProcessingText.SetActive(false);
       
        if(bOnlyClearProcessingText)
            return;

        fLastProgressForScaling = 0;
        fCurrentProgressForScaling = 0;
        fProgress = 0;

        Vector3 vNewScale = goProgressBar.transform.localScale;
        vNewScale.x = 0;
        goProgressBar.transform.localScale = vNewScale;
        
    }

    public void PlaceItemBackOntoShelf()
    {
        //Match it to the shelf height
        if(goShelfBeingPlacedOnTo != null)
        {
            Vector3 vNewPos = this.transform.position;
                    
            //Height
            foreach (Transform child in goShelfBeingPlacedOnTo.transform)
            {
                if (child.tag == Constants.ShelfHeightTag)
                {
                    vNewPos.y = child.gameObject.transform.position.y;
                    break;
                }
            }

            Vector3 shelfPos = goShelfBeingPlacedOnTo.transform.position;
            Vector3 shelfScale = goShelfBeingPlacedOnTo.transform.localScale;

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
                vNewPos.x = (shelfPos.x + (shelfScale.x / 2)) - this.transform.localScale.x;
            }
            else if(vNewPos.x < shelfPos.x - (shelfScale.x / 2))
            {
                vNewPos.x = (shelfPos.x - (shelfScale.x / 2)) + this.transform.localScale.x;
            }

            this.transform.position = vNewPos;
        }
    }
}
