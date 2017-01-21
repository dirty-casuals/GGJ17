using UnityEngine;

public class CustomerQueue : MonoBehaviour
{
    /// <summary>
    /// Returns the next customer or null if the queue is empty.
    /// </summary>    
    public CustomerAI GetCurrentCustomer()
    {
        return null;
    }

    /// <summary>
    /// Gets the rage of the queue (average);
    /// </summary>
    /// <returns></returns>
    public float GetQueueRage()
    {
        return 0;
    }

    /// <summary>
    /// Lets the customer go and brings the next one.
    /// </summary>    
    public void ReleaseCurrentCustomer()
    {
    }

    /// <summary>
    /// Returns true if the customer is in the que and sets his world space location and position in line
    /// Returns false if the cusomer is not in the queue;
    /// </summary>   
    public bool GetCustomerQueuLocation( CustomerAI customer, out Vector3 location, out int position )
    {
        position = -1;
        location = transform.position;
        return false;
    }
}
