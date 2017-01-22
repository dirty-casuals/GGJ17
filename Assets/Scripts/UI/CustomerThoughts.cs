using UnityEngine;

public class CustomerThoughts : MonoBehaviour
{
    [SerializeField]
    private CustomerAI aiHandle;
    [SerializeField]
    private GameObject thoughtBubble;
    [SerializeField]
    private GameObject angryFace;
    [SerializeField]
    private SpriteRenderer[] foodItems;
    private const float rageThreshold = 70.0f;
    private int currentItem = 0;

    private void Start()
    {
        SetupItemThoughts();
    }

    private void Update()
    {
        if(aiHandle == null)
        {
            return;
        }
        UpdateThoughtState();
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
        switch(aiHandle.currentState)
        {
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

    private void DisplayUnhappiness()
    {
        thoughtBubble.SetActive(true);
        angryFace.SetActive(true);
    }
}
