using UnityEngine;
using System.Collections.Generic;

public class AttackingState : UnitState
{
    private float _fightTimer;
    
    public RtsObject Target;
    
    public bool IsComplete
    {
        get
        {
            return true;
        }
    }
    public AttackingState(RtsObject attackObject)
    {
        Target = attackObject;
    }
    
    public void Enter(Unit unit)
    {
    }
    
    public void Update(Unit unit)
    {
        //If we don't have a target, find one.
            // bool hasTarget = false;
            // if (Target != null && Target.IsAlive && Target.CurrentState == UnitState.Fighting)
            // {
            //     hasTarget = true;

            //     if (_fightTimer >= LIFE_SECONDS)
            //     {
            //         LeanTween.cancel(gameObject);
            //         Injector.Get<GameState>().Kill(this);
            //         return;
            //     }
            // }

            // if (Target == null || !Target.gameObject.activeSelf)
            // {
            //     foreach (var other in UnitsInRange)
            //     {
            //         if (other == null)
            //         {
            //             //TODO: remove from list.
            //         }
            //         else
            //         {
            //             var otherUnit = other.GetComponent<Unit>();
            //             if (otherUnit != null && otherUnit.IsAlive && otherUnit.Team != Team)
            //             {
            //                 //We should fight!
            //                 UpdateState(UnitState.Fighting);
            //                 Target = otherUnit;
            //                 hasTarget = true;
            //                 _fightTimer = 0;

            //                 //Do some fighting thing.
            //                 LeanTween.scale(gameObject, Vector3.one, 1f)
            //                     .setEase(LeanTweenType.easeInOutElastic)
            //                     .setLoopPingPong();
            //                 break;
            //             }
            //         }

            //     }
                
            //     if (!hasTarget)
            // {
            //     _fightTimer = 0;
                
            //     //If we have no targets, go back to what we were doing.
            //     if (HasDestination)
            //     {
            //         UpdateState(UnitState.Moving);
            //     }
            //     else
            //     {
            //         UpdateState(UnitState.Idle);
            //     }
            // }

            // _fightTimer += Time.deltaTime;
    }
    
    public void Exit(Unit unit)
    {
        //TODO: Don't hardcode
        unit.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);   
        LeanTween.cancel(unit.gameObject);    
    }
}