using UnityEngine;
using UnityEngine.Assertions;
using System;

public class GatheringState : IUnitState
{
    private Mine _mine;
    private bool _isMining;

    private float _timer;

    private const float MINING_SECONDS_TOTAL = 2f;

    public bool IsComplete
    {
        get;
        private set;
    }

    public GatheringState(Mine mine)
    {
        _mine = mine;
    }

    public void Enter(Unit unit)
    {
        unit.Acceleration = Vector3.zero;
        _isMining = false;
    }

    public void Exit(Unit unit)
    {
        if (_isMining)
        {
            Assert.IsFalse(_mine.IsAvailable);
            _mine.EndMining(unit);
        }
    }

    public void Update(Unit unit)
    {
		// A little hacky, just lock the unit inside the mine while it's in this state.
		// Currently I can't see a scenario where this would cause issues, so don't overcomplicate.
        unit.transform.position = _mine.transform.position;
		
        // EARLY OUT
        if (unit.HasResource)
        {
            //Wouldn't expect this to happen with the current design.
            Debug.LogWarning("Unit finished mining from some external event");
            IsComplete = true;
            return;
        }
		// END EARLY OUT
		
        if (!_isMining && !unit.HasResource)
        {
            if (_mine.IsAvailable)
            {
                _isMining = true;
                _mine.BeginMining(unit);
            }
        }

        if (_isMining)
        {
            if (_timer >= MINING_SECONDS_TOTAL)
            {
                unit.HasResource = true;
                IsComplete = true;
            }
            _timer += Time.deltaTime;
        }
    }
}
