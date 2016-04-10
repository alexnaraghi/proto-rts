using UnityEngine;

public class OrbitingState : UnitState
{
    public RtsObject OrbitObject;
    private float _orbitTimer;
    private float _orbitAngle;
    
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
        _orbitTimer = 0f;
        _orbitAngle = 0f;
        
        //TODO: Probably get rid of this?  Depends if we keep the scaling effect or not on bases
        unit.transform.SetParent(OrbitObject.transform);
        
        var orbitPosition = GetOrbitPosition(unit.transform.position, OrbitObject.transform.position);
        unit.PushState(new MovingState(orbitPosition), true);
    }
    
    public void Update(Unit unit)
    {
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
        var sourceDir = (orbitCenter - source).normalized;
        return orbitCenter - sourceDir * 2.8f;
    }
    
    public void Exit(Unit unit)
    {
        unit.transform.SetParent(null);
    }
}
