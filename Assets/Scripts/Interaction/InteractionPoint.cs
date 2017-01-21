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
    private float fProgressTextBlinkTimer;


    private bool bInUse;

    private bool bCanBePlaced = false;
    public GameObject goShelfBeingPlacedOnTo;


    public Constants.InteractionPointType eInteractionType;
    public float fInteractionRadius = 0.5f;

    private SphereCollider sCollision;

	// Use this for initialization
	void Start () {
        sCollision = gameObject.AddComponent<SphereCollider>();

        sCollision.radius = fInteractionRadius;
        sCollision.isTrigger = true;

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
                if(other.GetComponent<PlayerController>().IsAbleToInteract())
                {
                    if (PlayerInput.QueryPlayerInput(Constants.InputType.PIT_INTERACT, true))
                    {
                        ProcessInteraction(other.gameObject);
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
    public void ResetProgress()
    {
        fLastProgressForScaling = 0;
        fCurrentProgressForScaling = 0;
        fProgress = 0;
        fProgressTextBlinkTimer = 0;
    }
}
