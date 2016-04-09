public interface UnitState
{
    bool IsComplete
    {
        get;
     }
    void Enter(Unit unit);
    void Update(Unit unit);
    void Exit(Unit unit);
}

