using UnityEngine;

public class OrbitingState : IUnitState
{
    private const float DESIRED_ORBIT_VELOCITY = 2.5f;

    private  RtsObject _orbitObject;
    private float _orbitRadius;

    //Some randomized variables so our orbits aren't completely uniform
    private float _personalDesiredVelocity;
    private float _personalRadius;
    
    
    private float _timoutSeconds;
    private bool _hasTimeout;

    private float _timerSeconds;

    public bool IsComplete
    {
        get
        {
            return _hasTimeout && _timerSeconds >= _timoutSeconds;
        }
    }
    public OrbitingState(RtsObject orbitObject, float orbitRadius)
    {
        _orbitObject = orbitObject;
        _orbitRadius = orbitRadius;
    }
    
    // We can also just orbit for a certain amount of time and stop.
    public OrbitingState(RtsObject orbitObject, float orbitRadius, float timeoutSeconds)
        : this(orbitObject, orbitRadius)
    {
        _hasTimeout = true;
        _timoutSeconds = timeoutSeconds;
    }
    
    public void Enter(Unit unit)
    {
        _personalDesiredVelocity = DESIRED_ORBIT_VELOCITY * Random.Range(0.8f, 1.2f);
        _personalRadius = _orbitRadius * Random.Range(0.8f, 1.2f);
    }
    
    public void Update(Unit unit)
    {
        if(unit.Aggro != null && unit.Aggro.Target != null)
        {
            unit.PushState(new AttackingState(unit.Aggro.Target), true);
            return;
        }
        
        var displacementToOrbitCenter = _orbitObject.transform.position - unit.transform.position;
        
        var distance = displacementToOrbitCenter.magnitude;
        
        // If we are right at the center, make up a direction
        if(distance < float.Epsilon)
        {
            displacementToOrbitCenter = new Vector3(0, 0, 1);
        }
        
        var perpVec = Vector3.Cross(displacementToOrbitCenter, Vector3.up);
        var tangentPoint = _orbitObject.transform.position + perpVec.normalized * _personalRadius;

        var velocity0 = unit.Velocity0.magnitude;

        Vector3 acceleration = Vector3.zero;
        
        // Don't go too fast to orbit
        if(velocity0 > _personalDesiredVelocity)
        {
            acceleration += -unit.Velocity0;
        }
        
        // The "correct" way to float around at a constant angular velocity is a = s^2 / r,
        // but it seems that on a timestep basis it makes more sense to just correct ourselves
        // along the tangent vector.
        var direction = (tangentPoint - unit.transform.position).normalized;
        var accelerationMagnitude = _personalDesiredVelocity;
        acceleration += direction * accelerationMagnitude;

        unit.Acceleration = acceleration;

        _timerSeconds += Time.deltaTime;
    }
    
    public void Exit(Unit unit)
    {
    }
}
