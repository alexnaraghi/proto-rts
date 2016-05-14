using UnityEngine;
using UnityEngine.Assertions;
using System;

public class UnloadingState : IUnitState
{
    private Base _base;
    private bool _isUnloading;

    private float _timer;

    private const float UNLOADING_SECONDS_TOTAL = 2f;

    public bool IsComplete
    {
        get;
        private set;
    }

    public UnloadingState(Base pBase)
    {
        _base = pBase;
    }

    public void Enter(Unit unit)
    {
        unit.Acceleration = Vector3.zero;
        _isUnloading = false;
    }

    public void Exit(Unit unit)
    {
        if (_isUnloading)
        {
            Assert.IsFalse(_base.IsAvailable);
            _base.EndUnloading(unit);
        }
    }

    public void Update(Unit unit, float deltaSeconds)
    {
		// A little hacky, just lock the unit inside the base while it's in this state.
		// Currently I can't see a scenario where this would cause issues, so don't overcomplicate.
        unit.transform.position = _base.transform.position;
		
        // EARLY OUT
        if (!unit.HasResource)
        {
            //Wouldn't expect this to happen with the current design.
            Debug.LogWarning("Unit finished mining from some external event");
            IsComplete = true;
            return;
        }
		// END EARLY OUT
		
        if (!_isUnloading && unit.HasResource)
        {
            if (_base.IsAvailable)
            {
                _isUnloading = true;
                _base.BeginUnloading(unit);
            }
        }

        if (_isUnloading)
        {
            if (_timer >= UNLOADING_SECONDS_TOTAL)
            {
                unit.HasResource = false;
                _base.AddResource(1);
                IsComplete = true;
            }
            _timer += deltaSeconds;
        }
    }
}
