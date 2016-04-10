using UnityEngine;
using System.Collections.Generic;

public class Unit : RtsObject
{
    
    private readonly UnitState _idleState = new IdlingState();

    public Stack<UnitState> StateStack;
    public List<GameObject> UnitsInRange;
    
    public bool HasResource;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        UnitsInRange = new List<GameObject>();
        StateStack = new Stack<UnitState>();
        
        //Idling is always on the bottom of the stack.
        PushState( _idleState);
        
        SetTeamColor(Team);
        IsAttackable = true;
    }

    void Update()
    {
        var top = StateStack.Peek();
        if(top != null && top.IsComplete)
        {
            Debug.Log("State popped: " + top.GetType().Name);
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

        if (CurrentState != UnitState.Fighting)
        {
            var otherUnit = other.GetComponent<Unit>();
            if (otherUnit != null && otherUnit.IsAlive && otherUnit.IsAttackable && otherUnit.Team != Team)
            {
                CurrentState = UnitState.Fighting;
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
    
    public void PushState(UnitState state)
    {
        PushState(state, false);
    }
    
    public void PushState(UnitState state, bool isChaining)
    {
        if(!isChaining)
        {
            while (StateStack.Count > 0)
            {
                var poppedState = StateStack.Pop();
                poppedState.Exit(this);
            }
            StateStack.Push(_idleState);
            Debug.Log("States cleared");
        }
        
        Debug.Log("State pushed, started:" + state.GetType().Name);
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
