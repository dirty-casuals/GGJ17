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

    private Vector3 vCustomInterUIPoint = new Vector3(-1, -1, -1);
    public Vector3 CustomInteraction
    {
        get { return vCustomInterUIPoint; }
    }

    private bool bInUse;

    private bool bCanBePlaced = false;
    private bool bShelfToPlaceDepthFlipped = false;
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

                goProgressBar.SetActive(false);
            }

            if (child.tag == Constants.IPProgressBarBGTag)
            {
                goProgressBarBG = child.gameObject;
                goProgressBarBG.SetActive(false);
            }

            if (child.tag == Constants.IPProgressTextTag)
            {
                goProcessingText = child.gameObject;
                goProcessingText.SetActive(false);
            }

            if(child.tag == Constants.CustomInteractionUIPointTag)
            {
                vCustomInterUIPoint = child.transform.position;
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
        if(other.tag == Constants.PlayerKillAreaTag)
        {
            Debug.Log("PlayerKillAreaTag");
            return;
        }

        if (!bInUse)
        {
            //Is a player near me?
            if (other.tag == Constants.PlayerTag)
            {
                PlayerController controllerHandle = other.GetComponent<PlayerController>();
                if(controllerHandle && controllerHandle.IsAbleToInteract() && controllerHandle.InteractionLockedTo == this)
                {
                    if (controllerHandle.QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
                    {
                        other.SendMessage(Constants.PlayerInteractionFunction, this.gameObject);
                    }
                }
            }
        }
        //in use and we're a food product, we're being carried and placed onto a shelf
        else if(eInteractionType == Constants.InteractionPointType.IPT_FOOD_PRODUCT)
        {
            if(other.tag == Constants.PlaceableShelfTag)
            {
                bCanBePlaced = true;
                bShelfToPlaceDepthFlipped = false;
            }
            else if(other.tag == Constants.PlaceableShelfTagFlippedDepth)
            {
                bCanBePlaced = true;
                bShelfToPlaceDepthFlipped = true;
            }

            
            if(bCanBePlaced)
            {
                goShelfBeingPlacedOnTo = other.gameObject;
            }
        }

        //cashier progress bar
        if (bInUse && other.tag == Constants.PlayerTag)
        {
            if(goProgressBar && goProgressBarBG)
            {
                if(!goProgressBar.activeSelf && !goProgressBarBG.activeSelf &&
                   eInteractionType == Constants.InteractionPointType.IPT_CASHIER_TILL)
                {
                    goProgressBar.SetActive(true);
                    goProgressBarBG.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if(bInUse)
        {
            bCanBePlaced = false;
        }

        if (other.tag == Constants.PlayerTag)
        {
            if(goProgressBar && goProgressBarBG)
            {
                if(goProgressBar.activeSelf && goProgressBarBG.activeSelf &&
                    eInteractionType == Constants.InteractionPointType.IPT_CASHIER_TILL)
                {
                    goProgressBar.SetActive(false);
                    goProgressBarBG.SetActive(false);
                }
            }
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
        if( bCanBePlaced )
        {
            GameObject[] allFoodstuff = GameObject.FindGameObjectsWithTag(Constants.FootItemTag);

            Vector3 vReturnPos = _GetReturnPlacement();

            for(int i = 0; i < allFoodstuff.Length; i++)
            {
                if(allFoodstuff[i] != this.gameObject)
                {
                    if(Vector3.Distance(allFoodstuff[i].transform.position, vReturnPos) < Constants.MinSpaceBetweenPlacedFoodItems)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        return false;
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

        if(goProgressBar)
            goProgressBar.SetActive(false);
        if(goProgressBarBG)
            goProgressBarBG.SetActive(false);
       
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
            Vector3 vNewPos = _GetReturnPlacement();

            this.transform.position = vNewPos;
        }
    }

    private Vector3 _GetReturnPlacement()
    {
        Vector3 vNewPos = this.transform.position;
        float fCustomDepthPos = 0;
        bool bHasCustomDepth = false;

        bool bFacingLeft = (goShelfBeingPlacedOnTo.GetComponent<PropFacingLeft>());
        bool bFacingRight = (goShelfBeingPlacedOnTo.GetComponent<PropFacingRight>());

        bool bTighterBounds = (goShelfBeingPlacedOnTo.GetComponent<TighterPlacementBounds>());
        

        //Height
        foreach (Transform child in goShelfBeingPlacedOnTo.transform)
        {
            if (child.tag == Constants.ShelfHeightTag)
            {
                vNewPos.y = child.gameObject.transform.position.y + this.GetComponent<BoxCollider>().size.z /2;
            }

            if(child.tag == Constants.ShelfDepthTag)
            {
                bHasCustomDepth = true;

                if(bFacingLeft || bFacingRight)
                    fCustomDepthPos = child.gameObject.transform.position.x;
                else
                    fCustomDepthPos = child.gameObject.transform.position.z;
            }
        }

        Vector3 shelfPos = goShelfBeingPlacedOnTo.transform.position;
        Vector3 shelfScale = goShelfBeingPlacedOnTo.GetComponent<BoxCollider>().size;

        //bShelfToPlaceDepthFlipped
        float shelfDepth = (bFacingLeft || bFacingRight ? shelfScale.x : shelfScale.z);

        //shelf depth positioning
        if(bFacingLeft || bFacingRight)
        {
            if(!bHasCustomDepth)
            {
                if(vNewPos.x < shelfPos.x - (shelfDepth * (bTighterBounds ? 0.365f : 0.5f)))
                {
                    vNewPos.x = shelfPos.x + shelfDepth * (bTighterBounds ? -0.01f : 0.125f);
                }
                else if(vNewPos.x > shelfPos.x + (shelfDepth * (bTighterBounds ? 0.365f : 0.5f)))
                {
                    vNewPos.x = shelfPos.x - shelfDepth * (bTighterBounds ? -0.01f : 0.125f);
                }
            }
            else
            {
                vNewPos.x = fCustomDepthPos;
            }
        }
        else
        {
            //top side of shelf
            if(!bHasCustomDepth)
            {
                if(vNewPos.z > shelfPos.z + (shelfDepth * (bTighterBounds ? 0.35f : 0.5f)))
                {
                    vNewPos.z = shelfPos.z + shelfDepth * (bTighterBounds ? 0.35f : 0.5f);
                }
                else if(vNewPos.z < shelfPos.z - (shelfDepth * (bTighterBounds ? 0.35f : 0.5f)))
                {
                    vNewPos.z = shelfPos.z - shelfDepth * (bTighterBounds ? 0.35f : 0.5f);
                }
            }
            else
            {
                vNewPos.z = fCustomDepthPos;
            }
        }
        
        //shelf width positioning
        if(bFacingLeft || bFacingRight)
        {
            if(vNewPos.z > shelfPos.z + (shelfScale.z * (bTighterBounds ? 0.35f : 0.5f)))
            {
                vNewPos.z = (shelfPos.z + (shelfScale.z * (bTighterBounds ? 0.35f : 0.5f)));
            }
            else if(vNewPos.z < shelfPos.z - (shelfScale.z * (bTighterBounds ? 0.35f : 0.5f)))
            {
                vNewPos.z = (shelfPos.z - (shelfScale.z * (bTighterBounds ? 0.35f : 0.5f)));
            }
        }
        else
        {
            //make sure the thing is on the shelf - more to the right
            if(vNewPos.x > shelfPos.x + (shelfScale.x * (bTighterBounds ? 0.35f : 0.5f)))
            {
                vNewPos.x = (shelfPos.x + (shelfScale.x * (bTighterBounds ? 0.35f : 0.5f)));
            }
            else if(vNewPos.x < shelfPos.x - (shelfScale.x * (bTighterBounds ? 0.35f : 0.5f)))
            {
                vNewPos.x = (shelfPos.x - (shelfScale.x * (bTighterBounds ? 0.35f : 0.5f)));
            }
        }

        return vNewPos;
    }
}
