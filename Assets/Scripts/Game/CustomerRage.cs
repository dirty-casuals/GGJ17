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
            if (child.tag == Constants.CustomerRageBarTag)
            {
                goRageBar = child.gameObject;
                sprRageBar = child.GetComponent<Sprite>();
                fRageBarMaxXScale = goRageBar.transform.localScale.x;

                Vector3 vNewScale = goRageBar.transform.localScale;
                vNewScale.x = 0;
                goRageBar.transform.localScale = vNewScale;

                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (aiHandle.IsInQueue())
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
            //redo this
            goRageBar.transform.localScale += new Vector3(fCurrentRageForScaling, 0, 0);
        }
	}

    public void Rage_FoundOutOfStockItem()
    {
        aiHandle.Rage += Constants.CustomerRageIncreaseOutOfStockItem;
    }
}
