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
    private GameObject deadFace;
    [SerializeField]
    private GameObject scaredFace;
    [SerializeField]
    private GameObject moneyThoughts;
    [SerializeField]
    private GameObject happyThoughts;
    [SerializeField]
    private SpriteRenderer[] foodItems;
    private const float rageThreshold = 70.0f;
    private GameObject previousCustomerMood;

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
        thoughtBubble.SetActive(false);
    }

    private void OnTriggerExit(Collider col)
    {
        thoughtBubble.SetActive(true);
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
            SetCustomerMood(deadFace);
            return;
        }
        switch(aiHandle.currentState)
        {
            case StateNames.Alerted:
                SetCustomerMood(scaredFace);
                break;
            case StateNames.TakeItem:
                CollectItem();
                break;
            case StateNames.WaitInQueue:
                if(aiHandle.Rage < rageThreshold)
                {
                    return;
                }
                SetCustomerMood(angryFace);
                break;
            case StateNames.Leave:
                if(aiHandle.Rage > 50.0f)
                {
                    return;
                }
                SetCustomerMood(happyThoughts);
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
            SetCustomerMood(moneyThoughts);
        }
    }

    private void SetCustomerMood(GameObject mood)
    {
        if(mood.activeInHierarchy)
        {
            return;
        }
        if(previousCustomerMood)
        {
            previousCustomerMood.SetActive(false);
        }
        previousCustomerMood = mood;
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
