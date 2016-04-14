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
        //TODO: Do some handling for if someone entered our attack range.


        var randomAcceleration = Random.Range(0, IDLING_ACCELERATION);

        var direction = (unit.transform.position - _pivotPosition).normalized;
        if (direction == Vector3.zero)
        {
            unit.Acceleration = new Vector3(1, -0.1f, 1) * randomAcceleration;
        }
        else
        {
            unit.Acceleration = -direction * randomAcceleration;
        }
    }

    public void Exit(Unit unit)
    {

    }
}
