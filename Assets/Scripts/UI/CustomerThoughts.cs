using UnityEngine;

public class CustomerThoughts : MonoBehaviour
{
    private PlayerController playerhandle;

    [SerializeField]
    private CustomerAI aiHandle;
    [SerializeField]
    private GameObject thoughtBubble;
    [SerializeField]
    private GameObject angryFace;
    [SerializeField]
    private GameObject deadFace;
    [SerializeField]
    private GameObject scaredFace;
    [SerializeField]
    private SpriteRenderer[] foodItems;
    private const float rageThreshold = 70.0f;

    private float fScaleTimer = 0;
    bool bBlowingUp, bShrinkingDown;

    private void Start()
    {
        playerhandle = GameObject.FindGameObjectWithTag(Constants.PlayerTag).GetComponent<PlayerController>();

        if(!playerhandle)
        {
            Debug.LogError("bad here");
            Debug.Break();
        }

        SetupItemThoughts();
    }

    private void Update()
    {
        if(aiHandle == null)
        {
            return;
        }
        UpdateThoughtState();

        if(!bBlowingUp)
        {
            bBlowingUp = playerhandle.QueryPlayerInput(Constants.InputType.PIT_CAMERA_ZOOM_OUT);
            if(bBlowingUp)
                fScaleTimer = 0;
        }
        
        if(bBlowingUp)
        {
            fScaleTimer += Time.deltaTime;

            if(this.transform.localScale.x < Constants.ZoomInBubbleBlowUpAmount)
            {
                float fScale = Constants.ZoomInBubbleBlowUpAmount * Constants.Normalise(fScaleTimer, 0, Constants.ZoomIconFadeInTime);
                fScale = Mathf.Max(1.0f, fScale);
                this.transform.localScale = new Vector3(fScale,fScale,fScale);
            }

            bShrinkingDown = !playerhandle.QueryPlayerInput(Constants.InputType.PIT_CAMERA_ZOOM_OUT);
            if(bShrinkingDown)
            {
                fScaleTimer = Constants.ZoomIconFadeInTime;
                bBlowingUp = false;
            }
        }

        if(bShrinkingDown)
        {
            fScaleTimer -= Time.deltaTime;

            if(this.transform.localScale.x > 1)
            {
                float fScale = 4.0f * Constants.Normalise(fScaleTimer, 0, Constants.ZoomIconFadeInTime);
                this.transform.localScale = new Vector3(fScale,fScale,fScale);
            }
            else bShrinkingDown = false;
        }

        
    }

    private void OnTriggerStay(Collider col)
    {
        if(aiHandle.currentState == StateNames.GotoItem)
        {
            thoughtBubble.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if(aiHandle.currentState == StateNames.GotoItem)
        {
            thoughtBubble.SetActive(true);
        }
    }

    private void SetupItemThoughts()
    {
        ShoppingItem[] items = aiHandle.targetItems;

        for(int i = 0; i < items.Length; i++)
        {
            foodItems[i].sprite = items[i].shoppingItemSprite;
            foodItems[i].gameObject.SetActive(true);
        }
    }

    private void UpdateThoughtState()
    {
        if(aiHandle.isDead)
        {
            DisplayDead();
            return;
        }
        switch(aiHandle.currentState)
        {
            case StateNames.Alerted:
                DisplayAlerted();
                break;
            case StateNames.TakeItem:
                CollectItem();
                break;
            case StateNames.WaitInQueue:
                if(aiHandle.Rage > rageThreshold)
                {
                    DisplayUnhappiness();
                }
                break;
        }
    }

    private void CollectItem()
    {
        int index = aiHandle.CurrentItemIdx;
        int itemList = aiHandle.targetItems.Length - 1;
        foodItems[index].gameObject.SetActive(false);

        if(index == itemList)
        {
            thoughtBubble.SetActive(false);
        }
    }

    private void DisplayDead()
    {
        SetCustomerMood(deadFace);
        angryFace.SetActive(false);
        scaredFace.SetActive(false);
    }

    private void DisplayAlerted()
    {
        SetCustomerMood(scaredFace);
        angryFace.SetActive(false);
    }

    private void DisplayUnhappiness()
    {
        SetCustomerMood(angryFace);
    }

    private void SetCustomerMood(GameObject mood)
    {
        if(mood.activeInHierarchy)
        {
            return;
        }
        HideAllItems();
        thoughtBubble.SetActive(true);
        mood.SetActive(true);
    }

    private void HideAllItems()
    {
        for(int i = 0; i < foodItems.Length; i++)
        {
            foodItems[i].gameObject.SetActive(false);
        }
    }
}
