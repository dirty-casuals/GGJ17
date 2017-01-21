using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerRage : MonoBehaviour
{
    private GameObject goRageBar;
    private Sprite sprRageBar;
    private float fRageBarMaxXScale;

    public bool bDebugDisableAmbientRageIncrease = false;

    

    private CustomerAI aiHandle;



    private float fTimer = 0.0f;
    
    

    private float fCurrentRageForScaling = 0;
    private float fLastRageForScaling = 0;

	// Use this for initialization
	void Start ()
    {
        aiHandle = this.GetComponent<CustomerAI>();

        foreach (Transform child in transform)
        {
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
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (aiHandle.IsInQueue() && !aiHandle.IsBeingServed())
        {
            fTimer += Time.deltaTime;

            if (fTimer > Constants.TickInterval && !bDebugDisableAmbientRageIncrease)
            {
                fTimer = 0.0f;
                aiHandle.Rage += Constants.CustomerRageInQueueIncreasePerTick;
            }
        }

        fLastRageForScaling = fCurrentRageForScaling;
        fCurrentRageForScaling = Mathf.Lerp(fCurrentRageForScaling, aiHandle.Rage, Constants.CustomerRageScaleFillRate * Time.deltaTime);

        if (fLastRageForScaling != fCurrentRageForScaling)
        {
            Vector3 vNewScale = goRageBar.transform.localScale;
            vNewScale.x = fRageBarMaxXScale * Constants.Normalise(fCurrentRageForScaling, 0, Constants.PlayerTillProgressToReach);
            goRageBar.transform.localScale = vNewScale;
        }
	}

    public void Rage_FoundOutOfStockItem()
    {
        aiHandle.Rage += Constants.CustomerRageIncreaseOutOfStockItem;
    }
}
