using UnityEngine;

public class AttackingState : IUnitState
{
    private float _fightTimer;
    
    private RtsObject _target;
    
    public bool IsComplete
    {
        get;
        private set;
    }
    public AttackingState(RtsObject attackObject)
    {
        _target = attackObject;
    }
    
    public void Enter(Unit unit)
    {
        Debug.Log("Unit is attacking");
    }
    
    public void Update(Unit unit)
    {
        //Decision: once a unit decides to attack, there's no turning back.
        
        if(!_target.IsAlive)
        {
            IsComplete = true;
            return;
        }

        Vector3 displacement = _target.transform.position - unit.transform.position;
        Vector3 moveTo;

        //HACK : TODO: Get rid of this stupid cast
        if(_target is Unit)
        {
            moveTo = displacement
                + (_target as Unit).Velocity0 
                - unit.Velocity0;            
        }
        else
        {
            moveTo = displacement;
        }

        if(displacement.magnitude < 0.5f)
        {
            unit.CreateExplosion();
            Injector.Get<GameState>().Kill(unit);
            Injector.Get<GameState>().Kill(_target);
            IsComplete = true;
            return;
        }
        
        Vector3 acceleration = Vector3.zero;

        var moveToUnit = (moveTo).normalized;
        var velocityUnit = unit.Velocity0.normalized;

        // If we are going way off from our destination, we need to dampen velocity and change course.
        var trajectoryDot = Vector3.Dot(moveToUnit, velocityUnit);
        if (trajectoryDot < 0.95f)
        {
            var correctionAcceleration = -unit.Velocity0 * Unit.MaxAcceleration;
        
            acceleration += correctionAcceleration;
        }

        //I think it probably makes sense to target where they are going, rather than where they are,
        //but we'll leave this for now since the mechanics probably are ok with this.
        acceleration += (displacement).normalized * Unit.MaxAcceleration;

        unit.Acceleration = acceleration;
    }
    
    public void Exit(Unit unit)
    {
        //TODO: Don't hardcode
        unit.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);   
        LeanTween.cancel(unit.gameObject);    
    }
}