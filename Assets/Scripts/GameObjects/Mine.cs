using UnityEngine.Assertions;
using UnityEngine;

public class Mine : RtsObject
{
    Base ClosestBase;

    public float RotationRate = 3f;

    float _radius;

    private Unit _gatheringUnit;

    protected override float _maxVelocity
    {
        get
        {
            return float.MaxValue;
        }
    }
    
    protected override float _maxAcceleration
    {
        get
        {
            return float.MaxValue;
        }
    }
    
    // Use this for initialization
    protected override void Start()
    {
        base.Start();


        //Right now mines and bases don't move, so cache the closest one on startup.
        ClosestBase = FindClosestBase();
        Assert.IsTrue(ClosestBase != null);
        
        _radius = (ClosestBase.transform.position - transform.position).magnitude;

        Assert.IsTrue(ClosestBase != null);
    }
    
    void Update()
    {
        var displacementToOrbitCenter = ClosestBase.transform.position - transform.position;
        var perpVectorUnit = Vector3.Cross(displacementToOrbitCenter, Vector3.up).normalized;
        
        Velocity0 = perpVectorUnit * RotationRate;
        Acceleration = displacementToOrbitCenter.normalized * (RotationRate * RotationRate / _radius);
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
    
    public bool IsAvailable
    {
        get {
            return _gatheringUnit == null;
        }
    }
    
    public void BeginMining(Unit unit)
    {
        Assert.IsNull(_gatheringUnit);
        _gatheringUnit = unit;
    }
    
    public void EndMining(Unit unit)
    {
        Assert.IsNotNull(_gatheringUnit);
        if (unit == _gatheringUnit)
        {
            _gatheringUnit = null;
        }
        else
        {
            Debug.LogWarning("Unit declared that it ended mining, but wasn't mining...");
        }
    }
}
