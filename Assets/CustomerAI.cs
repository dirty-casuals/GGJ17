using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    NavMeshAgent agent;

    ShoppingItem[] targetItems;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomItemTargets();
        agent.SetDestination( targetItems[0].position );
    }

    private void SetRandomItemTargets()
    {
        HashSet<ShoppingItem> itemIds = new HashSet<ShoppingItem>();

        int numTargets = Random.Range(2,3);
        while( itemIds.Count <= numTargets )
        {
            int id = Random.Range(0, ShoppingItem.items.Length-1 );
            itemIds.Add( ShoppingItem.items[id] );
        }

        targetItems = new ShoppingItem[itemIds.Count];
        itemIds.CopyTo( targetItems );
    }

    bool SeesItem( ShoppingItem item )
    {
        return Vector3.Distance( transform.position, item.position ) < 1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
