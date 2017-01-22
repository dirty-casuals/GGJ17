using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private CustomerQueue goCurrentQueueHandle;

    private GameObject goRageBar;
    private Sprite sprRageBar;
    private float fRageBarMaxXScale;
    private float fCurrentRageForScaling = 0;
    private float fLastRageForScaling = 0;

    private float fCalculateQueueRageTimer = 0;
    private float fLastQueueRage = 0;

	void Start ()
    {
        foreach( Transform child in transform )
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

        GameObject goTill = GameObject.FindGameObjectWithTag(Constants.TillTag);

        if(!goTill)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        goCurrentQueueHandle = goTill.GetComponent<CustomerQueue>();

        if(!goCurrentQueueHandle)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }
	}

    
	void Update ()
    {
        fCalculateQueueRageTimer += Time.deltaTime;
        if(fCalculateQueueRageTimer > Constants.TickInterval)
        {
            fCalculateQueueRageTimer = 0;
            fLastQueueRage = goCurrentQueueHandle.GetQueueRage();
        }



        fLastRageForScaling = fCurrentRageForScaling;
        fCurrentRageForScaling = Mathf.Lerp(fCurrentRageForScaling, fLastQueueRage, Constants.CustomerRageScaleFillRate * Time.deltaTime);

        if (fLastRageForScaling != fCurrentRageForScaling)
        {
            Vector3 vNewScale = goRageBar.transform.localScale;
            vNewScale.x = fRageBarMaxXScale * Constants.Normalise(fCurrentRageForScaling, 0, Constants.AvgQueueRageForGameFail);
            goRageBar.transform.localScale = vNewScale;
        }
	}
}
