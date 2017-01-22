using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject gameOverUIObject;
    public Text textGameOver;
    public RawImage textGameOverBg;

    private CustomerQueue goCurrentQueueHandle;

    private GameObject goRageBar;
    private Sprite sprRageBar;
    private float fRageBarMaxXScale;
    private float fCurrentRageForScaling = 0;
    private float fLastRageForScaling = 0;

    private float fCalculateQueueRageTimer = 0;
    private float fLastQueueRage = 0;

    private float fFadeInTimer = 0;


    void Awake()
    {
        Constants.bGameOver = false;
    }

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
        //if(Input.GetKeyDown(KeyCode.Z)) //take this out
         //   Constants.bGameOver = true;

        if(Constants.bGameOver)
        {
           // if(Input.GetKeyDown(KeyCode.Space))
           // {
           //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
           // }
        }
        else
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

            //Use the scaling one so it's linked to the UI display
            if(fCurrentRageForScaling >= Constants.AvgQueueRageForGameFail)
            {
                Constants.sFailReason = "Too much customer rage in the queue!";
                Constants.bGameOver = true;
            }
        }
	}
}
