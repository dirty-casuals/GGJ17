using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private GameObject goPlayerHandle;
    private GameObject goDesiredPosition;

	// Use this for initialization
	void Start () {
		goPlayerHandle = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

        if(!goPlayerHandle)
        {
            Debug.Assert(true, "Unable to find player GameObject! :(");
            Debug.Break();
        }
        else
        {
            goDesiredPosition = GameObject.FindGameObjectWithTag(Constants.PlayerCamPosTag);

            if(!goDesiredPosition)
            {
                Debug.Assert(true, "Unable to find player cam pos GameObject! :(");
                Debug.Break();
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		//PlayerCamFollowSpeed

        //this.transform.position 

        Vector3 vNewPos = this.transform.position;

        vNewPos.x = Mathf.Lerp(this.transform.position.x, goDesiredPosition.transform.position.x, Constants.PlayerCamFollowSpeed * Time.deltaTime);
        vNewPos.y = Mathf.Lerp(this.transform.position.y, goDesiredPosition.transform.position.y, Constants.PlayerCamFollowSpeed * Time.deltaTime);
        vNewPos.z = Mathf.Lerp(this.transform.position.z, goDesiredPosition.transform.position.z, Constants.PlayerCamFollowSpeed * Time.deltaTime);

        this.transform.position = vNewPos;
	}
}
