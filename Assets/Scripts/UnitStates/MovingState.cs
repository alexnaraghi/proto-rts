using UnityEngine;

public class MovingState : IUnitState
{
    private float _moveTimer;
    private float _timeToArrive;
    
    private Vector3 _startPosition;

    public const float MAX_VELOCITY = 7f;
    
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
        _moveTimer = 0f;
        _startPosition = unit.transform.position;
        _timeToArrive = (_destination - _startPosition).magnitude / MAX_VELOCITY;
    }
    
    public void Update(Unit unit)
    {
         float ratio = _moveTimer / _timeToArrive;
        unit.transform.position = (_startPosition * (1 - ratio)) + _destination * ratio;

        _moveTimer += Time.deltaTime;

        if (_moveTimer >= _timeToArrive)
        {
            IsComplete = true;
        }
    }
    
    public void Exit(Unit unit)
    {
        
    }
}
