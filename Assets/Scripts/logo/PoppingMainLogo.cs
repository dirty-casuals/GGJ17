using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppingMainLogo : MonoBehaviour {

    private Vector3 vStartingScale, vCurrentScale;

    private float fTimer;
    private float fTimeToCompleteFullScale = 0.7f;
    private float fMultiMod = 1.2f, fAimForMod = 1.1f;

    //1 = getting bigger, -1 = getting smaller
    private int iScaleDirection = 1;

	// Use this for initialization
	void Start () {
		vStartingScale = this.GetComponent<RectTransform>().localScale;
        vCurrentScale = vStartingScale;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(iScaleDirection == 1)
        {
            vCurrentScale = Vector3.Lerp(vCurrentScale, vStartingScale * fMultiMod, fTimeToCompleteFullScale * Time.deltaTime);
            this.GetComponent<RectTransform>().localScale = vCurrentScale;

            if(this.GetComponent<RectTransform>().localScale.x > vStartingScale.x * fAimForMod)
            {
                iScaleDirection = -1;
            }
        }
        else if(iScaleDirection == -1)
        {
            vCurrentScale = Vector3.Lerp(vCurrentScale, vStartingScale * 0.8f, fTimeToCompleteFullScale * Time.deltaTime);
            this.GetComponent<RectTransform>().localScale = vCurrentScale;

            if(this.GetComponent<RectTransform>().localScale.x <= vStartingScale.x * 0.9f)
            {
                iScaleDirection = 1;
            }
        }
	}
}
