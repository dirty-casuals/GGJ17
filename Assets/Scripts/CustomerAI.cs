using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateHandler
{
    public Object context { get; private set; }
    private Dictionary<string, AIState> stateMap;

    public string currentState { get; private set; }

    public StateHandler( Object context )
    {
        stateMap = new Dictionary<string, AIState>();
        this.context = context;
    }

    public void GotoState( string nextState )
    {
        string prevState = currentState;
        currentState = nextState;

        if( !string.IsNullOrEmpty( prevState ) )
            stateMap[prevState].OnExit();

        if( !string.IsNullOrEmpty( nextState ) )
            stateMap[nextState].OnEnter();

        Debug.LogFormat( "Going to state {0}", nextState );
    }

    public void AddState( AIState state )
    {
        stateMap[state.state] = state;
        state.stateHandler = this;
    }

    public void Update()
    {
        if( !string.IsNullOrEmpty( currentState ) )
        {
            stateMap[currentState].Update();
        }
    }
}

public abstract class AIState
{
    public StateHandler stateHandler { get; set; }
    public Object context { get { return stateHandler.context; } }

    public abstract string state { get; }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update() { }

    public virtual void GotoState( string state )
    {
        stateHandler.GotoState( state );
    }
}

public abstract class CustomerAIState : AIState
{
    public CustomerAI customer { get { return context as CustomerAI; } }
}

public static class StateNames
{
    public const string Init = "Init";
    public const string Wander = "Wander";
    public const string Alerted = "Alerted";
    public const string GotoItem = "GotoItem";
    public const string TakeItem = "TakeItem";
    public const string GotoTillQueue = "GotoTillQueue";
    public const string WaitInQueue = "WaitInQueue";
    public const string Leave = "Leave";
}

public class InitAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.Init; }
    }

    public override void OnEnter()
    {
        customer.SetRandomItemTargets();
        customer.SetFirstItem();
        GotoState( StateNames.GotoItem );
    }
}

public class GotoItemAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.GotoItem; }
    }

    public override void OnEnter()
    {
        customer.GoToCurrentItemExpectedLocation();
    }

    public override void Update()
    {
        if( customer.AtItemLocation() )
        {
            if( customer.SeesItem() )
            {
                GotoState( StateNames.TakeItem );
            }
            else
            {
                customer.SetNextExpectedItemLocation();
                GotoState( StateNames.GotoItem );
            }
        }
    }
}


public class TakeItemAIState : CustomerAIState
{
    private float itemTime = 0.5f;

    public override string state
    {
        get { return StateNames.TakeItem; }
    }

    public override void OnEnter()
    {
        customer.TakeItem();
        itemTime = 2.0f;
    }

    public override void Update()
    {
        if( itemTime > 0 )
        {
            itemTime -= Time.deltaTime;
            if( itemTime <= 0 )
            {
                MoveOn();
            }
        }
    }

    private void MoveOn()
    {
        customer.SetNextItem();

        if( customer.NeedsMoreItems() )
        {
            GotoState( StateNames.GotoItem );
        }
        else
        {
            GotoState( StateNames.GotoTillQueue );
        }
    }
}

public class AlertedAIState : CustomerAIState
{
    private float panicTimer = 2;
    public override string state
    {
        get { return StateNames.Alerted; }
    }

    public override void OnEnter()
    {
        panicTimer = Random.Range( 2.0f, 4.0f );
        customer.Panic();
    }

    public override void Update()
    {
        if( panicTimer > 0 )
        {
            panicTimer -= Time.deltaTime;
            if( panicTimer <= 0 )
            {
                GotoState( StateNames.Leave );
            }
        }
    }
}

public class GotoTillQueueAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.GotoTillQueue; }
    }

    public override void OnEnter()
    {
        CustomerQueue queue = GameObject.FindObjectOfType<CustomerQueue>();
        customer.GoToQueue( queue );
    }

    public override void Update()
    {
        if( !customer.IsInQueue() )
        {
            customer.UpdateQueueLocation();
        }
        else
        {
            GotoState( StateNames.WaitInQueue );
        }
    }
}

public class WaitInQueueAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.WaitInQueue; }
    }

    public override void Update()
    {
        if( customer.IsInQueue() )
        {
            customer.UpdateQueueLocation();
        }
        else
        {
            GotoState( StateNames.Leave );
        }
    }
}

public class LeaveStoreAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.Leave; }
    }

    public override void OnEnter()
    {
        customer.GoToGate();
    }

    public override void Update()
    {
        if( customer.AtGate() )
        {
            customer.LeaveBuilding();
        }
    }
}


public class CustomerAI : MonoBehaviour, IPawn
{
    public bool isDead { get; private set; }

    const float ALERT_RADIUS = 10;
    const float SIGHT_RADIUS = 3;
    const float DISTANCE_FROM_DESTINATION = 1.5f;

    StateHandler stateHandler;
    NavMeshAgent agent;

    ShoppingItem[] targetItems;
    int currentItemIdx = -1;
    CustomerQueue queue;

    float fCustomerRage = 0; //Current rage 0-100
    float fCustomerQueueTime = 0; //Time in the queue    

    public float Rage
    {
        get
        {
            return fCustomerRage;
        }
        set
        {
            fCustomerRage = value;
        }
    }

    public float TimeInQueue
    {
        get
        {
            return fCustomerQueueTime;
        }
    }

    public string currentState
    {
        get { return stateHandler.currentState; }
    }

    public ShoppingItem currentItem
    {
        get
        {
            return GetItem( currentItemIdx );
        }
    }

    public float speed
    {
        get
        {
            return agent.velocity.magnitude;
        }
    }

    public bool isHoldingItem
    {
        get
        {
            return false;
        }
    }

    public bool isPanicked { get; private set; }

    public Vector3 itemExtectedLocation;

    public event System.Action onPickupItem;
    public event System.Action onLeaveItem;
    public event System.Action onItemSwipe;

    private GameObject gate;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupStates();

        gate = GameObject.FindWithTag( "Gate" );
    }

    private void FixedUpdate()
    {
        if( Random.value < 0.000001f * Time.fixedDeltaTime )
        {
            Die(false); // of heart attack
        }
    }

    private void Update()
    {
        stateHandler.Update();
    }

    private void SetupStates()
    {
        stateHandler = new StateHandler( this );
        stateHandler.AddState( new InitAIState() );
        stateHandler.AddState( new AlertedAIState() );
        stateHandler.AddState( new GotoItemAIState() );
        stateHandler.AddState( new GotoTillQueueAIState() );
        stateHandler.AddState( new TakeItemAIState() );
        stateHandler.AddState( new WaitInQueueAIState() );
        stateHandler.AddState( new LeaveStoreAIState() );

        stateHandler.GotoState( StateNames.Init );
    }

    public void SetRandomItemTargets()
    {
        HashSet<ShoppingItem> itemIds = new HashSet<ShoppingItem>();

        ShoppingItem[] items = ShoppingItem.items;

        int numTargets = 1;//Random.Range(2,3);
        while( itemIds.Count < numTargets )
        {
            int id = Random.Range(0, items.Length-1 );
            itemIds.Add( items[id] );
        }

        targetItems = new ShoppingItem[itemIds.Count];
        itemIds.CopyTo( targetItems );
    }

    public bool AtItemLocation()
    {
        return AtItemLocation( currentItem );
    }

    private bool AtItemLocation( ShoppingItem item )
    {
        Vector3 charPosition = transform.position;
        Vector3 searchPosition = itemExtectedLocation;
        charPosition.y = 0;
        searchPosition.y = 0;
        return Vector3.Distance( charPosition, searchPosition ) < DISTANCE_FROM_DESTINATION;
    }

    public bool SeesItem()
    {
        return SeesItem( currentItem );
    }

    bool SeesItem( ShoppingItem item )
    {
        Vector3 charPosition = transform.position;
        Vector3 itemPosition = item.position;
        charPosition.y = 0;
        itemPosition.y = 0;
        bool isInSight = Vector3.Distance( charPosition, itemPosition ) < SIGHT_RADIUS;
        bool isPickable = item.CanBePickedUp();
        bool isOnShelf = item.IsOnShelf();
        bool isInStock = item.Quantity > 0;

        if( !isInStock )
        {
            GetComponent<CustomerRage>().Rage_FoundOutOfStockItem();
        }
        return isInSight && isPickable && isOnShelf && isInStock;
    }

    public bool NeedsMoreItems()
    {
        return currentItemIdx < targetItems.Length;
    }

    public void GoToCurrentItemExpectedLocation()
    {
        agent.SetDestination( itemExtectedLocation );
    }

    public void SetFirstItem()
    {
        currentItemIdx = 0;
        if( currentItem )
        {
            itemExtectedLocation = currentItem.originalPlace;
        }
    }

    public void SetNextItem()
    {
        currentItemIdx += 1;
        if( currentItem )
        {
            itemExtectedLocation = currentItem.originalPlace;
        }
    }

    private ShoppingItem GetItem( int itemIndex )
    {
        if( itemIndex >= 0 && itemIndex < targetItems.Length )
            return targetItems[itemIndex];
        else
            return null;
    }

    public void TakeItem()
    {
        if( currentItem )
            currentItem.Quantity--;

        if( onPickupItem != null )
        {
            onPickupItem();
        }
    }

    internal void SetNextExpectedItemLocation()
    {
        int idx = Random.Range( 0, ShoppingItem.items.Length );
        ShoppingItem item = ShoppingItem.items[idx];
        itemExtectedLocation = item.position;
    }

    public void GoToQueue( CustomerQueue queue )
    {
        this.queue = queue;
        UpdateQueueLocation();
    }

    public void UpdateQueueLocation()
    {
        Vector3 targetLocation;
        int palceInQueue;

        bool isInQueue = queue.GetCustomerQueueLocation( this, out targetLocation, out palceInQueue );
        agent.SetDestination( targetLocation );

        if( !isInQueue && Vector3.Distance( transform.position, targetLocation ) <= 1 )
        {
            queue.AddCustomer( this );
            fCustomerQueueTime = 0;
        }

        fCustomerQueueTime += Time.deltaTime;
    }

    public bool IsInQueue()
    {
        return queue != null && queue.Contains( this );
    }

    public bool IsBeingServed()
    {
        //Current customer and is player interacting
        return (queue.GetCurrentCustomer() == this && queue.IsPlayerUsingTill());
    }

    public void GoToGate()
    {        
        agent.SetDestination( gate.transform.position );
    }

    public bool AtGate()
    {
        return Vector3.Distance( transform.position, gate.transform.position ) <= 2;
    }

    public void LeaveBuilding()
    {
        Destroy( gameObject );
    }

    public void Die( bool alert )
    {
        isDead = true;
        agent.Stop();
        stateHandler.GotoState( null );

        if( queue != null && queue.Contains( this ) )
        {
            queue.ReleaseCustomer( this );
        }

        var rag = GetComponentInChildren<Ragdoll>();
        if( rag )
        {
            rag.SetRagdoll( true );
        }

        StartCoroutine( DestroyAfterTime( 10 ) );

        if( alert )
        {
            AlertEveryone();
        }
    }

    private void AlertEveryone()
    {
        Collider[] colliders = Physics.OverlapSphere( transform.position, ALERT_RADIUS );
        foreach( var collider in colliders )
        {
            CustomerAI ai = collider.GetComponent<CustomerAI>();
            if( ai!=null && ai != this )
            {
                Vector3 rayDir = ai.transform.position - transform.position;

                // Line of sight
                RaycastHit hit;
                if( Physics.Raycast( transform.position, rayDir, out hit ) )
                {
                    if( hit.collider.transform.root == collider.transform.root )
                    {
                        ai.Alert();
                    }
                }
            }
        }
    }

    private void Alert()
    {
        if( stateHandler.currentState != StateNames.Alerted )
        {
            stateHandler.GotoState( StateNames.Alerted );
        }
    }

    private IEnumerator DestroyAfterTime( int time )
    {
        yield return new WaitForSeconds( time );
        Destroy( gameObject );
    }

    public void Leave()
    {
        if( queue != null && queue.Contains( this ) )
        {
            queue.ReleaseCustomer( this );
        }
    }

    internal void Panic()
    {
        isPanicked = true;
        agent.ResetPath();
    }
}
