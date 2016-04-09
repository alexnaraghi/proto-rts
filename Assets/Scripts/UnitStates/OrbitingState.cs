using UnityEngine;
using System.Collections;


public class OrbitingState : UnitState
{
    public RtsObject OrbitObject;
    private float _orbitTimer;
    private float _orbitAngle;
    
    public bool IsComplete
    {
        get
        {
            return true;
        }
    }
    public OrbitingState(RtsObject orbitObject)
    {
        OrbitObject = orbitObject;
    }
    
    public void Enter(Unit unit)
    {
        
            //     _orbitTimer = 0f;
            //     _orbitAngle = 0f;
            //     OrbitObject = objectToOrbit;
            // gameObject.transform.SetParent(objectToOrbit.gameObject.transform);
            // UpdateState(UnitState.Orbiting);
    }
    
    public void Update(Unit unit)
    {
        // const float RADIUS_PER_SECOND = 2f;
        //         _orbitAngle += RADIUS_PER_SECOND * _orbitTimer;
        //         gameObject.transform.Rotate(OrbitObject.transform.position, _orbitAngle, Space.Self);
                
        //         _orbitTimer += Time.deltaTime;
    }
    
    public void Exit(Unit unit)
    {
        
    }
}
