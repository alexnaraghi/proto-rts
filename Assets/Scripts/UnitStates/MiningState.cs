using UnityEngine;
using System.Collections;
public class MiningState : IUnitState
{
    public enum MineSubState
    {
        MovingToMine,
        Mining,
        MovingToBase,
        Unloading
    }

    public Base Base;
    public Mine Mine;

    public MineSubState SubState;

    private float _timer;

    const float MINE_SECONDS = 4f;
    const float UNLOAD_SECONDS = 4f;

    public bool IsComplete
    {
        get;
        private set;
    }

    public MiningState(Base pBase, Mine mine)
    {
        Base = pBase;
        Mine = mine;
    }

    public void Enter(Unit unit)
    {
        //Everything is handled in the update loop.
    }

    public void Update(Unit unit, float deltaSeconds)
    {
        if(Base.Team != unit.Team)
        {
            IsComplete = true;
            return;
        }
        
        if (unit.Aggro != null && unit.Aggro.Target != null)
        {
            unit.PushState(new AttackingState(unit.Aggro.Target), true);
            return;
        }

        //Delegate out a sub-action for mining.
        if (unit.HasResource)
        {
            if (IsInsideBase(unit))
            {
                unit.PushState(new UnloadingState(Base), true);
            }
            else
            {
                unit.PushState(new MovingState(Base), true);
            }
        }
        else
        {
            if (IsInsideMine(unit))
            {
                unit.PushState(new GatheringState(Mine), true);
            }
            else
            {
                unit.PushState(new MovingState(Mine), true);
            }
        }
    }

    public void Exit(Unit unit)
    {

    }

    private bool IsInsideMine(Unit unit)
    {
        return (Mine.transform.position - unit.transform.position).magnitude < 1f;
    }

    private bool IsInsideBase(Unit unit)
    {
        return (Base.transform.position - unit.transform.position).magnitude < 1f;
    }

    private bool IsNearBase(Unit unit)
    {
        return (Base.transform.position - unit.transform.position).magnitude < Base.ORBIT_RADIUS;
    }

    private Vector3 ClosestPointToBaseOrbit(Unit unit)
    {
        //Get right inside the orbit.
        var displacement = Base.transform.position - unit.transform.position + Vector3.one;

        var magnitude = displacement.magnitude;
        var magnitudeMinusOrbit = Mathf.Abs(magnitude - Base.ORBIT_RADIUS / 2);

        return unit.transform.position + displacement.normalized * magnitudeMinusOrbit;
    }

    private bool IsNearMine(Unit unit)
    {
        return (Mine.transform.position - unit.transform.position).magnitude < 1f;
    }
}
