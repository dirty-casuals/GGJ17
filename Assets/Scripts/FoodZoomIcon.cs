using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodZoomIcon : MonoBehaviour {

    private PlayerController controllerHandle;
    private SpriteRenderer srChildIconSprite;

    private ShoppingItem shopItemHandle;
    private TextMesh outOfStockPrompt;

    private float fOutOfStockTimer = 0;
    private float fBlinkingTimer = 0;
    private float fFadeInTimer = 0;

    private float fCurrentBlinkDecrease = 0;

    private bool bDisplayingIcon = false;
    private bool bLastDisplayingIcon = false;

	// Use this for initialization
	void Start () {
		controllerHandle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        shopItemHandle = gameObject.GetComponent<ShoppingItem>();
        srChildIconSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        foreach( Transform child in transform )
        {
            if(child.tag == Constants.ShopItemOutOfStockTag)
            {
                outOfStockPrompt = child.GetComponent<TextMesh>();
            }
        }

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

        if(!shopItemHandle)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        if(!outOfStockPrompt)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }

        Color col = srChildIconSprite.color;
        col.a = 0;
        srChildIconSprite.color = col;

        col = outOfStockPrompt.color;
        col.a = 0;
        outOfStockPrompt.color = col;
	}
	
	
	void Update ()
    {
        if(Constants.bGameOver)
        {
            return;
        }

        if(shopItemHandle.Quantity == 0)
        {
            fOutOfStockTimer += Time.deltaTime;

            if(fOutOfStockTimer > 0.2f)
            {
                fOutOfStockTimer = 0;

                if(outOfStockPrompt.color.a == 1)
                {
                    Color col = outOfStockPrompt.color;
                    col.a = 0.0f;
                    outOfStockPrompt.color = col;
                }
                else
                {
                    Color col = outOfStockPrompt.color;
                    col.a = 1;
                    outOfStockPrompt.color = col;
                }
            }
        }
        else
        {
            fOutOfStockTimer = 0;
            if(outOfStockPrompt.color.a != 0)
            {
                Color col = outOfStockPrompt.color;
                col.a = 0;
                outOfStockPrompt.color = col;
            }
        }

        //fOutOfStockTimer



        bLastDisplayingIcon = bDisplayingIcon;
        bDisplayingIcon = controllerHandle.QueryPlayerInput(Constants.InputType.PIT_CAMERA_ZOOM_OUT);

        if(bLastDisplayingIcon != bDisplayingIcon)
        {
            fFadeInTimer = 0;
        }

        if(bDisplayingIcon)
        {
            fFadeInTimer += Time.deltaTime;

            if(shopItemHandle.Quantity == 0)
            {
                fBlinkingTimer += Time.deltaTime;

                if(fBlinkingTimer > 0.2f)
                {
                    fBlinkingTimer = 0;

                    if(fCurrentBlinkDecrease == 0)
                        fCurrentBlinkDecrease = 0.5f;
                    else
                        fCurrentBlinkDecrease = 0;
                }
            }

            if(fFadeInTimer < Constants.ZoomIconFadeInTime)
            {
                Color col = srChildIconSprite.color;
                col.a = (1.0f * Constants.Normalise(fFadeInTimer, 0, Constants.ZoomIconFadeInTime)) - fCurrentBlinkDecrease;
                srChildIconSprite.color = col;
            }
            else if(shopItemHandle.Quantity == 0)
            {
                Color col = srChildIconSprite.color;
                col.a = Mathf.Lerp(col.a, 1.0f - fCurrentBlinkDecrease, 0.2f);
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
