using UnityEngine;

public class IdlingState : IUnitState
{
    private const float IDLING_ACCELERATION = 1.5f;
    private Vector3 _pivotPosition;
    public IdlingState()
    {

    }

    public bool IsComplete
    {
        get
        {
            return false;
        }
    }

    public void Enter(Unit unit)
    {
        _pivotPosition = unit.transform.position;
    }

    public void Update(Unit unit)
    {
        if(unit.Aggro != null && unit.Aggro.Target != null)
        {
            unit.PushState(new AttackingState(unit.Aggro.Target), true);
            return;
        }
        
        var randomAcceleration = Random.Range(0, IDLING_ACCELERATION);

        var acceleration = Vector3.zero;

        // slow down idling dudes who are moving fast from another state
        if(unit.Velocity0.sqrMagnitude > 0.3f)
        {
            acceleration += -unit.Velocity0;
        }
        
        //Do some little movement to make idling interesting
        var direction = (unit.transform.position - _pivotPosition).normalized;
        if (direction == Vector3.zero)
        {
            acceleration += new Vector3(1, -0.1f, 1) * randomAcceleration;
        }
        else
        {
            acceleration += -direction * randomAcceleration;
        }

        unit.Acceleration = acceleration;
    }

    public void Exit(Unit unit)
    {

    }
}
