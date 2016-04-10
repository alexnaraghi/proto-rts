using UnityEngine;

public class OrbitingState : IUnitState
{
    public RtsObject OrbitObject;
    private float _orbitTimer;
    private float _orbitAngle;
    
    private bool _isFirstUpdate;
    
    public bool IsComplete
    {
        get
        {
            return false;
        }
    }
    public OrbitingState(RtsObject orbitObject)
    {
        OrbitObject = orbitObject;
    }
    
    public void Enter(Unit unit)
    {
        _isFirstUpdate = true;
        _orbitTimer = 0f;
        _orbitAngle = 0f;
        
        var orbitPosition = GetOrbitPosition(unit.transform.position, OrbitObject.transform.position);
        unit.PushState(new MovingState(orbitPosition), true);
    }
    
    public void Update(Unit unit)
    {
        if(_isFirstUpdate)
        {
            //TODO: Probably get rid of this?  Depends if we keep the scaling effect or not on bases
            unit.transform.SetParent(OrbitObject.transform);
            _isFirstUpdate = false;
        }
        /*
        const float RADIUS_PER_SECOND = 2f;
        
        _orbitAngle += RADIUS_PER_SECOND * _orbitTimer;
        var axis = unit.transform.position - OrbitObject.transform.position;
        unit.transform.Rotate(axis, _orbitAngle, Space.Self);
          */
                
        _orbitTimer += Time.deltaTime;
    }
    
    public Vector3 GetOrbitPosition(Vector3 source, Vector3 orbitCenter)
    {
        var offset = orbitCenter - source;
        
        // If the object is at the center, we need to spit it out
        // in some non-zero direction.
        if(offset.sqrMagnitude < 0.001f)
        {
            offset = new Vector3(1f, 0f, 1f);
        }
        
        var sourceDir = offset.normalized;
        return orbitCenter - sourceDir * 2.8f;
    }
    
    public void Exit(Unit unit)
    {
        unit.transform.SetParent(null);
    }
}
