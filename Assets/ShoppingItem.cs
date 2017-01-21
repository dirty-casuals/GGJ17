using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItem : MonoBehaviour
{
    public static ShoppingItem[] items;

    public Vector3 originalPlace;

    public string itemName { get { return gameObject.name; } }

    public Vector3 position { get { return transform.position; } }

    private int itemQuantity;
    public int Quantity
    {
        get
        {
            return itemQuantity;
        }

        set
        {
            itemQuantity = value;
        }
    }

    void Awake()
    {
        originalPlace = transform.position;

        if( items == null || items.Length == 0 || items[0] == null )
        {
            items = FindObjectsOfType<ShoppingItem>();
        }
    }
}
