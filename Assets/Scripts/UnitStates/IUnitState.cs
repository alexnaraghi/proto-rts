public interface IUnitState
{
    bool IsComplete
    {
        get;
     }
    void Enter(Unit unit);
    void Update(Unit unit, float deltaSeconds);
    void Exit(Unit unit);
}

