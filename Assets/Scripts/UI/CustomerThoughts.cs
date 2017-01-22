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

    private void Start()
    {
        SetupItemThoughts();
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

    private void Update()
    {
        if(aiHandle == null)
        {
            return;
        }
        DisplayUnhappiness();
    }

    private void OnTriggerStay(Collider col)
    {
        thoughtBubble.SetActive(false);
    }

    private void OnTriggerExit(Collider col)
    {
        thoughtBubble.SetActive(true);
    }

    private void DisplayUnhappiness()
    {
        if(aiHandle.Rage > rageThreshold && !angryFace.activeSelf)
        {
            angryFace.SetActive(true);
            for(int i = 0; i < foodItems.Length; i++)
            {
                foodItems[i].gameObject.SetActive(false);
            }
        }
    }
}
