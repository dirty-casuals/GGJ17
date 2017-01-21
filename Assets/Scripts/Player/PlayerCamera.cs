using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private GameObject goPlayerHandle;
    private PlayerController playerScriptHandle;

	void Start () {
		goPlayerHandle = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

        if(!goPlayerHandle)
        {
            Debug.Assert(true, "Unable to find player GameObject! :(");
            Debug.Break();
        }

        playerScriptHandle = goPlayerHandle.GetComponent<PlayerController>();
	}
	//
	void Update ()
    {
        if(!playerScriptHandle.QueryPlayerInput(Constants.InputType.PIT_CAMERA_ZOOM_OUT))
        {
            Vector3 vNewPos = this.transform.position;

            vNewPos.x = Mathf.Lerp(this.transform.position.x, goPlayerHandle.transform.position.x,                                 Constants.PlayerCamFollowSpeed * Time.deltaTime);
            vNewPos.y = Mathf.Lerp(this.transform.position.y, goPlayerHandle.transform.position.y + Constants.PlayerCameraOffsetY, Constants.PlayerCamFollowSpeed * Time.deltaTime);
            vNewPos.z = Mathf.Lerp(this.transform.position.z, goPlayerHandle.transform.position.z + Constants.PlayerCameraOffsetZ, Constants.PlayerCamFollowSpeed * Time.deltaTime);

            this.transform.position = vNewPos;
        }
        else
        {
            Vector3 vNewPos = this.transform.position;

            
            vNewPos.y = Mathf.Lerp(this.transform.position.y, goPlayerHandle.transform.position.y + Constants.PlayerCameraZoomOutOffsetY, Constants.PlayerCamFollowSpeed * Time.deltaTime);


            if(playerScriptHandle.QueryPlayerInput(Constants.InputType.PIT_UP))
            {
                vNewPos.z += Constants.CameraSpeedZ * Time.deltaTime;
            }
            else if(playerScriptHandle.QueryPlayerInput(Constants.InputType.PIT_DOWN))
            {
                vNewPos.z -= Constants.CameraSpeedZ * Time.deltaTime;
            }

            if(playerScriptHandle.QueryPlayerInput(Constants.InputType.PIT_LEFT))
            {
                vNewPos.x -= Constants.CameraSpeedX * Time.deltaTime;
            }
            else if(playerScriptHandle.QueryPlayerInput(Constants.InputType.PIT_RIGHT))
            {
                vNewPos.x += Constants.CameraSpeedX * Time.deltaTime;
            }






            this.transform.position = vNewPos;
        }
	}
}
