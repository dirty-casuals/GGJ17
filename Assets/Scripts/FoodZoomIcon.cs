using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodZoomIcon : MonoBehaviour {

    private PlayerController controllerHandle;
    private SpriteRenderer srChildIconSprite;

    private float fFadeInTimer = 0;
    private bool bDisplayingIcon = false;
    private bool bLastDisplayingIcon = false;

	// Use this for initialization
	void Start () {
		controllerHandle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        srChildIconSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        if(!controllerHandle)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        if(!srChildIconSprite)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        Color col = srChildIconSprite.color;
        col.a = 0;
        srChildIconSprite.color = col;
	}
	
	
	void Update ()
    {
        if(Constants.bGameOver)
        {
            return;
        }

        bLastDisplayingIcon = bDisplayingIcon;
        bDisplayingIcon = controllerHandle.QueryPlayerInput(Constants.InputType.PIT_CAMERA_ZOOM_OUT);

        if(bLastDisplayingIcon != bDisplayingIcon)
        {
            fFadeInTimer = 0;
        }

        if(bDisplayingIcon)
        {
            fFadeInTimer += Time.deltaTime;

            if(fFadeInTimer < Constants.ZoomIconFadeInTime)
            {
                Color col = srChildIconSprite.color;
                col.a = 1.0f * Constants.Normalise(fFadeInTimer, 0, Constants.ZoomIconFadeInTime);
                srChildIconSprite.color = col;
            }
        }
        else
        {
            Color col = srChildIconSprite.color;

            if(col.a != 0)
            {
                fFadeInTimer += Time.deltaTime;
                
                col.a = 1.0f * (1.0f - Constants.Normalise(fFadeInTimer, 0, Constants.ZoomIconFadeInTime));
                srChildIconSprite.color = col;
            }
        }
	}
}
