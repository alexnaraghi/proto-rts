using UnityEngine;

public class MovingState : IUnitState
{
    private float _moveTimer;
    private float _timeToArrive;
    
    private Vector3 _startPosition;
    
    Vector3 _destination;
    
    public bool IsComplete
    {
        get;
        protected set;
    }
    
    public MovingState(Vector3 destination)
    {
        _destination = destination;
    }
    
    public void Enter(Unit unit)
    {
        //_moveTimer = 0f;
        _startPosition = unit.transform.position;
        //_timeToArrive = (_destination - _startPosition).magnitude / MAX_VELOCITY;
    }
    
    public void Update(Unit unit)
    {
        if(unit.Aggro != null && unit.Aggro.Target != null)
        {
            unit.PushState(new AttackingState(unit.Aggro.Target), true);
            return;
        }
        
        var displacement = _destination - unit.transform.position;
        var distance = displacement.magnitude;
        
        var direction = displacement.normalized;
        var velocity0 = unit.Velocity0.magnitude;
        var unitVelocity0 = unit.Velocity0.normalized;

        //var vectorWeDontWant = displacement - unit.Velocity0;

        var brakeTime = velocity0 / Unit.MaxAcceleration;
        var projectedArrivalTime = distance / velocity0;

        Vector3 acceleration = Vector3.zero;
    
        //Figure out how far off our velocity is from our direction
        var trajectoryDot = Vector3.Dot(unitVelocity0, direction);

        // If we are going way off from our destination, we need to dampen velocity and change course.
        if (trajectoryDot < 0.95f)
        {
            var correctionAcceleration = -unit.Velocity0 * Unit.MaxAcceleration;
        
            acceleration = correctionAcceleration;
        }

        if(projectedArrivalTime > brakeTime)
        {
            acceleration += (direction) * Unit.MaxAcceleration;
        }
        else
        {
            //As soon as we are slowed to a crawl, call it done.
            if(distance < 1f && Vector3.Dot(unit.Velocity0, displacement) < 0 && velocity0 < 3f)
            {
                IsComplete = true;
                acceleration = Vector3.zero;
            }
            else
            {
                acceleration += direction * -Unit.MaxAcceleration;
            }
        }
        
        unit.Acceleration = acceleration;
    }
    
    public void Exit(Unit unit)
    {
        
    }
}
