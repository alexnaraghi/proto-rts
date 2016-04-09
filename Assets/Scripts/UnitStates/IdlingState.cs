public class IdlingState : UnitState
{
    public IdlingState()
    {
        
    }
    
    public bool IsComplete
    {
        get {return false;}
    }
    
    public void Enter(Unit unit)
    {
    }
    
    public void Update(Unit unit)
    {
        //Do some handling for if someone entered our attack range.
    }
    
    public void Exit(Unit unit)
    {
        
    }
}
