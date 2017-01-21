using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private GameObject goPlayerHandle;

	void Start () {
		goPlayerHandle = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

        if(!goPlayerHandle)
        {
            Debug.Assert(true, "Unable to find player GameObject! :(");
            Debug.Break();
        }
	}
	
	void Update ()
    {
        Vector3 vNewPos = this.transform.position;

        vNewPos.x = Mathf.Lerp(this.transform.position.x, goPlayerHandle.transform.position.x,                                 Constants.PlayerCamFollowSpeed * Time.deltaTime);
        vNewPos.y = Mathf.Lerp(this.transform.position.y, goPlayerHandle.transform.position.y + Constants.PlayerCameraOffsetY, Constants.PlayerCamFollowSpeed * Time.deltaTime);
        vNewPos.z = Mathf.Lerp(this.transform.position.z, goPlayerHandle.transform.position.z + Constants.PlayerCameraOffsetZ, Constants.PlayerCamFollowSpeed * Time.deltaTime);

        this.transform.position = vNewPos;
	}
}
