using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private const int SIGHT_RADIUS = 1;
    private const int DISTANCE_FROM_DESTINATION = 1;
    NavMeshAgent agent;

    ShoppingItem[] targetItems;

    private int currentItemIdx = -1;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomItemTargets();
        GotoNextTarget();

    }

    private void GotoNextTarget()
    {
        currentItemIdx++;
        if( currentItemIdx < targetItems.Length )
        {
            agent.SetDestination( targetItems[currentItemIdx].position );
        }
        else if( currentItemIdx == targetItems.Length )
        {
            GameObject till = GameObject.FindGameObjectWithTag( "Checkout" );
            agent.SetDestination( till.transform.position );
        }
        else
        {
            GameObject exit = GameObject.FindGameObjectWithTag( "Gate" );
            agent.SetDestination( exit.transform.position );
        }
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

    bool SeesItem()
    {        
        ShoppingItem targetItem = targetItems[currentItemIdx];
        return SeesItem( targetItem );
    }

    bool SeesItem( ShoppingItem item )
    {
        return Vector3.Distance( transform.position, item.position ) < SIGHT_RADIUS;
    }

    private bool LookingForItem()
    {
        return currentItemIdx < targetItems.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if( agent.remainingDistance <= DISTANCE_FROM_DESTINATION )
        {
            if( LookingForItem() && SeesItem() )
            {
                GotoNextTarget();
            }
            else if( !LookingForItem() )
            {
                GotoNextTarget();
            }
        }
    }
}
