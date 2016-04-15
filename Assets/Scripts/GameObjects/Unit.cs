using UnityEngine;
using System.Collections.Generic;

public class Unit : RtsObject
{
    public const float MaxVelocity = 10f;
    public const float MaxAcceleration = 5f;
    private const float FRICTION_COEFICIENT = 4f;
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

    public Vector3 Acceleration;
    public Vector3 Velocity0
    {
        get;
        private set;
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
        IUnitState top = null;
        if (StateStack.Count > 0)
        {
            top = StateStack.Peek();
        }
        if (top != null && top.IsComplete)
        {
            DebugStateLog.Add("State popped: " + top.GetType().Name);
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

    void LateUpdate()
    {
        Acceleration = Vector3.ClampMagnitude(Acceleration, MaxAcceleration);

        //I think we need to do some friction here to make the unit at rest at really low speeds..
        if (Velocity0.magnitude < 2f && Acceleration.magnitude < 0.01f)
        {
            var friction = -FRICTION_COEFICIENT * Velocity0;
            Acceleration += friction;
        }

        var position0 = transform.position;
        var positionDelta = 0.5f * Acceleration * (Time.deltaTime * Time.deltaTime)
                             + Velocity0 * Time.deltaTime;
        var position = position0 + positionDelta;

        // Apply forces
        transform.position = position;

        // Prepare for next frame
        Velocity0 = Velocity0 + Acceleration * Time.deltaTime;
        Velocity0 = Vector3.ClampMagnitude(Velocity0, MaxVelocity);

        Acceleration = Vector3.zero;
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
    
    public bool HasAggro
    {
        get
        {
            return UnitsInRange.Count > 0;
        }
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
        if (!isChaining)
        {
            while (StateStack.Count > 0)
            {
                var poppedState = StateStack.Pop();
                poppedState.Exit(this);
            }
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
    
    public void CreateExplosion()
    {
        if(ExplosionPrefab != null)
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
