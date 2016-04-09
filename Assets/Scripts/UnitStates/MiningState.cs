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
    
    private Base _base;
    private Mine _mine;
    
    public MineSubState SubState;
    
    private float _timer;
    
    const float MINE_SECONDS = 4f;
    const float UNLOAD_SECONDS = 4f;

    public bool IsComplete
    {
        get
        {
            return true;
        }
    }

    public MiningState(Base pBase, Mine mine)
    {
        _base = pBase;
        _mine = mine;
    }
    
    public void Enter(Unit unit)
    {
        if(unit.HasResource)
        {
            SubState = MineSubState.MovingToBase;
        }
        else
        {
            SubState = MineSubState.MovingToMine;
        }
    }
    
    public void Update(Unit unit)
    {
    }
    
    public void Exit(Unit unit)
    {
        
    }
    
    private bool IsAtMine()
    {
        return false;
    }
    
    private bool IsAtBase()
    {
        return false;
    }
}
