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
        get
        {
            return false;
        }
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

    public void Update(Unit unit)
    {
        if (unit.Aggro != null && unit.Aggro.Target != null)
        {
            unit.PushState(new AttackingState(unit.Aggro.Target), true);
            return;
        }

        //Delegate out a sub-action for mining.
        if (unit.HasResource)
        {
           // if (IsInsideBase(unit) && !Base.IsUnitInside())
            if (IsInsideBase(unit))
            {
                //TODO: Make a state for collecting a resource
                Base.AddResource(1);
                unit.HasResource = false;
            }
            // else if (IsNearBase(unit))
            // {
            //     if (!Base.IsUnitInside())
            //     {
            //         unit.PushState(new MovingState(Base.transform.position), true);
            //         Base.UnitInside = unit;
            //     }
            //     else
            //     {
            //         unit.PushState(new OrbitingState(unit, Base.ORBIT_RADIUS, 2f));
            //     }
            // }
            // else
            // {
            //     var insideBaseRange = ClosestPointToBaseOrbit(unit);
            //     unit.PushState(new MovingState(insideBaseRange), true);
            // }
            else
            {
                unit.PushState(new MovingState(Base), true);
            }
        }
        else
        {
            // if (Base.IsUnitInside() && Base.UnitInside.Id == unit.Id)
            // {
            //     Base.UnitInside = null;
            // }
            // else if (IsInsideMine(unit))
            // {
            //     //TODO: Make a state for collecting a resource
            //     unit.HasResource = true;
            // }
            if (IsInsideMine(unit))
            {
                //TODO: Make a state for collecting a resource
                unit.HasResource = true;
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
