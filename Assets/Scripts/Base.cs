using System.Collections.Generic;

public class Base : RtsObject 
{
    public List<Unit> OrbitingObjects;
    
    protected override void Start()
    {
        base.Start();
        IsTargetable = true;
        IsAttackable = false;
    }
    
    public override void OnTargeted(Unit targettingObject, bool isChaining)
    {
        if(targettingObject != null)
        {
            targettingObject.QueueState(new OrbitingState(this));
        }
    }

}
