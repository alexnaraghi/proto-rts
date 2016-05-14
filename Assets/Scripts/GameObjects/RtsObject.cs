using UnityEngine;

public class RtsObject : MonoBehaviour
{
    public int Id;
    public SelectableObject MySelectableObject;
    
    public bool IsAlive;
    
    //Can be attacked and killed.
    public bool IsAttackable;
    
    //Can be targetted and moved to
    public bool IsTargetable;
    
    public int Team;
    
    public Vector3 Acceleration;
    public Vector3 Velocity0
    {
        get;
        protected set;
    }
    
    protected virtual float _maxVelocity
    {
        get
        {
            return 0f;
        }
    }
    protected virtual float _maxAcceleration
    {
        get
        {
            return 0f;
        }
    }
    
    protected virtual float _frictionCoefficient
    {
        get
        {
            return 0f;
        }
    }
    
    protected virtual void Awake()
    {
        MySelectableObject = GetComponent<SelectableObject>();
    }
    
    protected virtual void Start()
    {
        IsAlive = true;
    }
    
    public virtual void GameUpdate(float deltaSeconds)
    {
        
    }    
    
    public virtual void LateGameUpdate(float deltaSeconds)
    {
        if (_maxVelocity > float.Epsilon)
        {
            Acceleration = Vector3.ClampMagnitude(Acceleration, _maxAcceleration);

            //I think we need to do some friction here to make the unit at rest at really low speeds..
            /*
            if (Velocity0.magnitude < 2f && Acceleration.magnitude < 0.01f)
            {
                var friction = -_frictionCoefficient * Velocity0;
                Acceleration += friction;
            }*/

            var position0 = transform.position;
            var positionDelta = 0.5f * Acceleration * (deltaSeconds)
                                 + Velocity0 * deltaSeconds;
            var position = position0 + positionDelta;

            // Apply forces
            transform.position = position;

            // Bound the unit position to the map bounds.
            //
            // We need to bound velocity as well, so units don't get "caught" on the border of the map.
            // or maybe we should just prevent this entirely through the selection mechanic.
            // I'll just use this for now for testing so we don't get objects flying outside the camera
            // bounds.
            var bounds = Injector.Get<GameState>().MapBounds;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -bounds.x, bounds.x),
                                             Mathf.Clamp(transform.position.y, -bounds.y, bounds.y),
                                             Mathf.Clamp(transform.position.z, -bounds.z, bounds.z));

            // Prepare for next frame
            Velocity0 = Velocity0 + Acceleration * deltaSeconds;
            Velocity0 = Vector3.ClampMagnitude(Velocity0, _maxVelocity);

            Acceleration = Vector3.zero;
        }
    }
    
    public bool IsSelectable
    {
        get
        {
            return MySelectableObject != null;
        }
    }
    
    public virtual void OnTargeted(Unit attacker, bool isChaining)
    {
        //Do nothing
    }
    
    public bool Equals (RtsObject obj)
    {
        if (obj == null || Id != obj.Id)
        {
            return false;
        }
        
        return true;
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
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchWhiteAlbedo");
                break;
            case 4:
                resource = Resources.Load<Material>(MATERIALS_PATH + "SwatchNavyAlbedo");
                break;
            default:
                Debug.Log("This team doesn't have an associated color. " + teamNumber);
                break;

        }
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        
        if(renderer != null)
        {
            renderer.material = resource;
        }
    }
}
