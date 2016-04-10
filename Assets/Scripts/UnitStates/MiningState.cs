using UnityEngine;
using System.Collections;
public class MiningState : UnitState
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
        if(!unit.HasResource && IsAtMine(unit))
        {
            //TODO: Make a state for collecting a resource
            unit.HasResource = true;
        }
        
        if(unit.HasResource && IsAtBase(unit))
        {
            //TODO: Make a state for collecting a resource
            unit.HasResource = false;
        }
            
        if(unit.HasResource)
        {
            unit.PushState(new MovingState(Base.transform.position), true);
        }
        else
        {
            unit.PushState(new MovingState(Mine.transform.position), true);            
        }
    }
    
    public void Exit(Unit unit)
    {
        
    }
    
    private bool IsAtMine(Unit unit)
    {
        return (Mine.transform.position - unit.transform.position).magnitude < 1f;
    }
    
    private bool IsAtBase(Unit unit)
    {
        return (Base.transform.position - unit.transform.position).magnitude < 1f;
    }
}
