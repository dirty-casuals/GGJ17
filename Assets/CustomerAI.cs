using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateHandler
{
    public Object context { get; private set; }
    private Dictionary<string, AIState> stateMap;

    private string currentState;

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
    public const string GotoItem = "GotoItem";
    public const string GotoTillQueue = "GotoTillQueue";
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
                customer.TakeItem();
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
            else
            {
                customer.SetNextExpectedItemLocation();
                GotoState( StateNames.GotoItem );
            }
        }
    }
}


public class GotoTillQueueAIState : CustomerAIState
{
    public override string state
    {
        get { return StateNames.GotoItem; }
    }

    public override void OnEnter()
    {
        CustomerQueue queue = GameObject.FindObjectOfType<CustomerQueue>();
        customer.GoToQueue( queue );
    }

    public override void Update()
    {
        if( customer.IsInQueue() )
        {
            customer.UpdateLocationInQueue();
        }
        else
        {
            GotoState( StateNames.Leave );
        }
    }
}


public class CustomerAI : MonoBehaviour
{
    const int SIGHT_RADIUS = 1;
    const int DISTANCE_FROM_DESTINATION = 1;
    StateHandler stateHandler;
    NavMeshAgent agent;

    ShoppingItem[] targetItems;
    int currentItemIdx = -1;
    CustomerQueue queue;


    private ShoppingItem currentItem
    {
        get
        {
            return GetItem( currentItemIdx );
        }
    }

    public Vector3 itemExtectedLocation;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupStates();
    }

    private void Update()
    {
        stateHandler.Update();
    }

    private void SetupStates()
    {
        stateHandler = new StateHandler( this );
        stateHandler.AddState( new InitAIState() );
        stateHandler.AddState( new GotoItemAIState() );
        stateHandler.AddState( new GotoTillQueueAIState() );

        stateHandler.GotoState( StateNames.Init );
    }

    public void SetRandomItemTargets()
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

    public bool AtItemLocation()
    {
        return AtItemLocation( currentItem );
    }

    private bool AtItemLocation( ShoppingItem item )
    {
        return Vector3.Distance( transform.position, itemExtectedLocation ) < SIGHT_RADIUS;
    }

    public bool SeesItem()
    {
        return SeesItem( currentItem );
    }

    bool SeesItem( ShoppingItem item )
    {
        return Vector3.Distance( transform.position, item.position ) < SIGHT_RADIUS;
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
        // pretend this does a thing
        // TODO: actually take item from stock
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
        if( queue.AddCustomer( this ) )
        {
            UpdateLocationInQueue();
        }
    }

    public void UpdateLocationInQueue()
    {
        Vector3 targetLocation;
        int palceInQueue;

        if( queue.GetCustomerQueueLocation( this, out targetLocation, out palceInQueue ) )
        {
            agent.SetDestination( targetLocation );
        }
    }

    public bool IsInQueue()
    {
        return queue != null && queue.Contains( this );
    }
}
