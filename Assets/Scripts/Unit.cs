using UnityEngine;
using System.Collections.Generic;

public class Unit : RtsObject
{
    
    private readonly IUnitState _idleState = new IdlingState();

    public Stack<IUnitState> StateStack;
    public List<GameObject> UnitsInRange;

    //I want to keep track of all state transitions for checking a unit in the editor.
    //Maybe make this a buffer if memory is a concern?
    public List<string> DebugStateLog = new List<string>();

    public bool HasResource;
    
    protected override void Awake()
    {
        base.Awake();
        UnitsInRange = new List<GameObject>();
        StateStack = new Stack<IUnitState>();
        
        //Idling is always on the bottom of the stack.
        PushState( _idleState);
        
        SetTeamColor(Team);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        var top = StateStack.Peek();
        if(top != null && top.IsComplete)
        {
            DebugStateLog.Add("State popped: " + top.GetType().Name);
            top.Exit(this);
            StateStack.Pop();
            top = StateStack.Peek();
        }
        
        if(top != null)
        {
            top.Update(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        /*
        UnitsInRange.Add(other.gameObject);

        if (CurrentState != IUnitState.Fighting)
        {
            var otherUnit = other.GetComponent<Unit>();
            if (otherUnit != null && otherUnit.IsAlive && otherUnit.IsAttackable && otherUnit.Team != Team)
            {
                CurrentState = IUnitState.Fighting;
            }
        }
        */
    }

    void OnTriggerExit(Collider other)
    {
        /*
        UnitsInRange.Remove(other.gameObject);

        if (other.gameObject == Target)
        {
            Target = null;
        }*/
        
    }
    
    public void SetTeamColor(int teamNumber)
    {
        const string MATERIALS_PATH = "Materials/";
        Material resource = null;
        switch (teamNumber)
        {
            case 1:
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchPinkDAlbedo");
                break;
            case 2:
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchTealAlbedo");
                break;
            case 3:
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchNavyAlbedo");
                break;
            case 4:
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchWhiteAlbedo");
                break;
            default:
                Debug.Log("This team doesn't have an associated color. " + teamNumber);
                break;

        }
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = resource;
        }
    }
    
    public void PushState(IUnitState state)
    {
        PushState(state, false);
    }
    
    public void PushState(IUnitState state, bool isChaining)
    {
        if(!isChaining)
        {
            while (StateStack.Count > 0)
            {
                var poppedState = StateStack.Pop();
                poppedState.Exit(this);
            }
            StateStack.Push(_idleState);
        }
        DebugStateLog.Add("State pushed, started:" + state.GetType().Name);
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
}
