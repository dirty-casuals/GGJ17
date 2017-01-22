using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomerQueue : MonoBehaviour
{
    Transform queueStart;
    float customerSpacing = 1.4f;

    List<CustomerAI> customerQueue = new List<CustomerAI>();


    private void Start()
    {
        queueStart = transform.FindChild( "QueueStart" );

        if( queueStart == null )
        {
            queueStart = transform;
        }
    }

    public bool IsPlayerUsingTill()
    {
        return this.GetComponent<InteractionPoint>().InUse();
    }

    /// <summary>
    /// Returns the queue the player deals with
    /// </summary>
    /// <returns></returns>
    public static CustomerQueue GetPlayerOperatedQueue()
    {
        return FindObjectOfType<CustomerQueue>();
    }

    /// <summary>
    /// Returns the next customer or null if the queue is empty.
    /// </summary>    
    public CustomerAI GetCurrentCustomer()
    {
        if( customerQueue.Count > 0 )
        {
            return customerQueue[0];
        }

        return null;
    }

    /// <summary>
    /// Gets the rage of the queue (average);
    /// </summary>
    /// <returns></returns>
    public float GetQueueRage()
    {
        if( customerQueue.Count > 0 )
        {
            float fAverageRage = 0;
            int iTimesAddedToRage = 0;
            foreach(CustomerAI ai in customerQueue)
            {
                fAverageRage += ai.Rage;
                iTimesAddedToRage++;
            }

            return (fAverageRage / iTimesAddedToRage);
        }
        return 0;
    }

    /// <summary>
    /// Lets the customer go and brings the next one.
    /// </summary>
    public void ReleaseCurrentCustomer()
    {
        if( customerQueue.Count > 0 )
        {
            customerQueue.RemoveAt( 0 );
        }
    }

    public float GetCurrentCustomerRage()
    {
        if( customerQueue.Count > 0 )
        {
            return customerQueue[0].Rage;
        }
        return -1.0f;
    }
    public float GetCurrentCustomerTimeInQueue()
    {
        if( customerQueue.Count > 0 )
        {
            return customerQueue[0].TimeInQueue;
        }
        return -1.0f;
    }

    /// <summary>
    /// Let the specified customer go.
    /// </summary>
    public void ReleaseCustomer( CustomerAI customerAI )
    {
        customerQueue.Remove( customerAI );
    }

    /// <summary>
    /// Returns true if the customer is in the que and sets his world space location and position in line
    /// Returns false if the cusomer is not in the queue;
    /// </summary>   
    public bool GetCustomerQueueLocation( CustomerAI customer, out Vector3 location, out int position )
    {
        position = customerQueue.FindIndex( c => { return c == customer; } );
        bool isInQueue = position >= 0; 

        if( !isInQueue )
        {
            position = GetCustomerCount();

        }

        location = queueStart.position + queueStart.forward * position * customerSpacing;
        return isInQueue;
    }


    /// <summary>
    /// Adds a customer to the queue. Returns true if the customer could be added;
    /// </summary>
    public bool AddCustomer( CustomerAI customer )
    {
        customerQueue.Add( customer );        
        return true;
    }

    /// <summary>
    /// Returns if the customer is in the queue.
    /// </summary>
    public bool Contains( CustomerAI customer )
    {
        return customerQueue.Contains( customer );
    }

    /// <summary>
    /// Returns the number of customers in the queue
    /// </summary>
    public int GetCustomerCount()
    {
        return customerQueue.Count;
    }    
}
