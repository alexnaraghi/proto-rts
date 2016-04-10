using UnityEngine.Assertions;
using UnityEngine;

public class Mine : RtsObject
{
    Base ClosestBase;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //Right now mines and bases don't move, so cache the closest one on startup.
        ClosestBase = FindClosestBase();
        Assert.IsTrue(ClosestBase != null);
    }

    public override void OnTargeted(Unit targettingUnit, bool isChaining)
    {
        if(targettingUnit != null)
        {
            targettingUnit.PushState(new MiningState(ClosestBase, this), isChaining);
        }
    }


    private Base FindClosestBase()
    {
        Base closest = null;
        float distance = float.MaxValue;
        
        var bases = GameObject.FindObjectsOfType<Base>();

        //TODO: Optimize this junk
        foreach (var baseObj in bases)
        {
            if (baseObj != null)
            {
                //use square magnitude, why not.  it's faster
                var currentDist = (gameObject.transform.position - baseObj.gameObject.transform.position).sqrMagnitude;
                if (currentDist < distance)
                {
                    distance = currentDist;
                    closest = baseObj;
                }
            }
        }
        return closest;
    }
}
