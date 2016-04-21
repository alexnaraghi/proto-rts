using UnityEngine;
using System.Collections.Generic;

public class Unit : RtsObject
{
    
    private readonly IUnitState _idleState = new IdlingState();

    public Stack<IUnitState> StateStack;
    public List<GameObject> UnitsInRange;

    public GameObject ExplosionPrefab;

    public Aggro Aggro
    {
        get; 
        private set;
    }

    //I want to keep track of all state transitions for checking a unit in the editor.
    //Maybe make this a buffer if memory is a concern?
    public List<string> DebugStateLog = new List<string>();

    public bool HasResource;

    public const float MaxVelocity = 5f;
    public const float MaxAcceleration = 2f;
    public const float FrictionCoefficient = 4f;


    //I think these should be in some kind of data, not inheritance.
    protected override float _maxVelocity
    {
        get
        {
            return MaxVelocity;
        }
    }
    
    protected override float _maxAcceleration
    {
        get
        {
            return MaxAcceleration;
        }
    }
    protected override float _frictionCoefficient
    {
        get
        {
            return FrictionCoefficient;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        UnitsInRange = new List<GameObject>();
        StateStack = new Stack<IUnitState>();

        Aggro = GetComponentInChildren<Aggro>();

        SetTeamColor(Team);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        _idleState.Enter(this);
    }

    void Update()
    {
        if(!IsAlive)
        {
            return;
        }
        
        IUnitState top = null;
        if (StateStack.Count > 0)
        {
            top = StateStack.Peek();
        }
        if (top != null && top.IsComplete)
        {
            //DebugStateLog.Add("State popped: " + top.GetType().Name);
            top.Exit(this);
            StateStack.Pop();
            
            if(StateStack.Count > 0)
            {
                top = StateStack.Peek();
            }
            else
            {
                _idleState.Enter(this);
            }
        }

        if (top != null)
        {
            top.Update(this);
        }
        else
        {
            _idleState.Update(this);
        }
    }
    
    public bool HasAggro
    {
        get
        {
            return UnitsInRange.Count > 0;
        }
    }

    public void PushState(IUnitState state)
    {
        PushState(state, false);
    }

    public void PushState(IUnitState state, bool isChaining)
    {
        if (!isChaining)
        {
            while (StateStack.Count > 0)
            {
                var poppedState = StateStack.Pop();
                poppedState.Exit(this);
            }
        }
        //DebugStateLog.Add("State pushed, started:" + state.GetType().Name);
        StateStack.Push(state);
        state.Enter(this);
    }

    /*
    //Do something when ordered
    public override void OnOrder(Vector3 destination)
    {
        QueueState(new MovingState(destination));      
    }
    
    //Do something when ordered
    public override void OnOrder(RtsObject destinationObject)
    {
        if(destinationObject.IsTargetable)
        {
            destinationObject.OnTargeted(this);
        }
    }*/

    public override void OnTargeted(Unit attacker, bool isChaining)
    {
        attacker.PushState(new AttackingState(this), isChaining);
    }
    
    public void CreateExplosion()
    {
        if(ExplosionPrefab != null)
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
