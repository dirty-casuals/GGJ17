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
    private int iQuantity = 3;
    private int iOldQuantity = 3;
    public int Quantity
    {
        get {
            return iQuantity;
        }
        set {
            iQuantity = value;
        }
    }

    void Awake()
    {
        originalPlace = transform.position;
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
