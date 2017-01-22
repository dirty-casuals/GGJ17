using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItem : MonoBehaviour
{
    public static ShoppingItem[] items { get { return FindObjectsOfType<ShoppingItem>(); } }

    public Vector3 originalPlace;

    public string itemName { get { return gameObject.name; } }

    public Vector3 position { get { return transform.position; } }

    public Sprite shoppingItemSprite;

    private TextMesh TextGameObject;
    private int iQuantity = Constants.ShopItemStartingAmount;
    private int iOldQuantity = 3; //make this 3 so it will update the text
    public int Quantity
    {
        get {
            return iQuantity;
        }
        set {
            iQuantity = value;
        }
    }

    public bool IsOnShelf()
    {
        return (!this.GetComponent<InteractionPoint>().InUse());
    }

    public bool CanBePickedUp()
    {
        return (IsOnShelf() && Quantity >= 1);
    }

    void Start()
    {
        foreach( Transform child in transform )
        {
            if( child.tag == Constants.FoodTextQuantityTag )
            {
                TextGameObject = child.GetComponent<TextMesh>();
                break;
            }

            originalPlace = transform.position;
        }

        if(!TextGameObject)
        {
            Debug.LogError("failed here!");
            Debug.Break();
        }
    }

    void Update()
    {
        if(iOldQuantity != iQuantity)
        {
            iOldQuantity = iQuantity;
            TextGameObject.text = iQuantity.ToString();
        }
    }
}
