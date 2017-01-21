using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRage : MonoBehaviour
{
    public float fPlayerRage = Constants.PlayerStartingRage;
    public float fPlayerScore = 0;


    public bool bDebugDisableAmbientRageIncrease = false;

    private float fTimer = 0.0f;

	void Start ()
    {
		
	}
	
	void Update ()
    {
		fTimer += Time.deltaTime;

        if(fTimer > Constants.TickInterval && !bDebugDisableAmbientRageIncrease)
        {
            fTimer = 0.0f;
            fPlayerRage += Constants.PlayerRageIncreasePerTick;
        }
	}

    private float NormalisedWaitingTime(float waitTime)
    {
        return ( (waitTime - 0.0f) / (Constants.MaxCustomerWaitTime - 0) );
    }


    public void Score_ProcessedCustomerItems(float fCustomerRage, float fCustomerWaitingTime)
    {
        //When putting items thru, the max score is decreased by customer rage and time waited
        fPlayerScore += Constants.PlayerScoreIncreaseForProcessingItems - (fCustomerRage * (1 - Constants.Normalise(fCustomerWaitingTime, 0, Constants.MaxCustomerWaitTime) ));
    }


    public void Rage_ProcessedCustomerItems()
    {
        fPlayerRage += Constants.PlayerRageIncreaseProcessedItems;
    }

    public void Rage_KilledCustomer()
    {
        fPlayerRage -= Constants.PlayerRageDecreaseKilledCustomer;
    }
}
