using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItem : MonoBehaviour
{
    public static ShoppingItem[] items;

    public Vector3 originalPlace;

    public string itemName { get { return gameObject.name; } }

    public Vector3 position { get { return transform.position; } }

    void Start()
    {
        originalPlace = transform.position;

        if( items == null || items.Length == 0 || items[0] == null )
        {
            items = FindObjectsOfType<ShoppingItem>();
        }
    }
}
