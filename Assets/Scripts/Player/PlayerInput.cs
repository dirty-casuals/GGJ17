using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    


	// Use this for initialization
	void Start () {
		
	}
	
	void Update ()
    {
        
	}

    public static bool QueryPlayerInput(Constants.InputType eType, bool bJustPressed = false)
    {
        switch(eType)
        {
            case Constants.InputType.PIT_UP: {  return (!bJustPressed) ? Input.GetKey(Constants.upKey) : Input.GetKeyDown(Constants.upKey); }
            case Constants.InputType.PIT_DOWN: {  return (!bJustPressed) ? Input.GetKey(Constants.downKey) : Input.GetKeyDown(Constants.downKey); }
            case Constants.InputType.PIT_LEFT: {  return (!bJustPressed) ? Input.GetKey(Constants.leftKey) : Input.GetKeyDown(Constants.leftKey); }
            case Constants.InputType.PIT_RIGHT: {  return (!bJustPressed) ? Input.GetKey(Constants.rightKey) : Input.GetKeyDown(Constants.rightKey); }

            case Constants.InputType.PIT_INTERACT: {  return (!bJustPressed) ? Input.GetKey(Constants.interactionKey) : Input.GetKeyDown(Constants.interactionKey); }
            case Constants.InputType.PIT_ATTACK: {  return (!bJustPressed) ? Input.GetKey(Constants.attackKey) : Input.GetKeyDown(Constants.attackKey); }
        }

        return false;
    }
}
