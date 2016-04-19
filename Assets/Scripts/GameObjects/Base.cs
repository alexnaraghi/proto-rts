using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class Base : RtsObject
{
    public const float ORBIT_RADIUS = 25f;
    public Unit ProductionUnitPrefab;
    
    public List<Unit> OrbitingObjects;

    //public Unit UnitInside;
    
    public int Resource;
    
    public bool isProducing;

    const float UNIT_PRODUCTION_SECONDS = 0.2f;
    
    private float _productionSeconds;
    
    protected override void Start()
    {
        base.Start();
        
        Assert.IsTrue(ProductionUnitPrefab != null);
    }
    
    void Update()
    {
        if(!isProducing && Resource > 0)
        {
            Resource--;
            isProducing = true;
        }
        
        if(isProducing)
        {
            if(_productionSeconds >= UNIT_PRODUCTION_SECONDS)
            {
                CompleteUnit();
                isProducing = false;
                _productionSeconds = 0;
            }
            else
            {
                _productionSeconds += Time.deltaTime;
            }
        }
    }
    
    public override void OnTargeted(Unit targettingObject, bool isChaining)
    {
        if(targettingObject != null)
        {
            targettingObject.PushState(new OrbitingState(this, ORBIT_RADIUS));
        }
    }
    
    public void AddResource(int resourceCount)
    {
        Resource += resourceCount;
    }
    
    public bool CanAddResource()
    {
        return Resource < 1;
    }

    // public bool IsUnitInside()
    // {
    //     return UnitInside != null;
    // }

    private void CompleteUnit()
    {
        var unit = Injector.Get<GameObjectFactory>().CreateRtsObject(ProductionUnitPrefab);
        
        //TODO: use the right team
        unit.Team = 1;
        unit.transform.position = transform.position;
        unit.PushState(new OrbitingState(this, ORBIT_RADIUS), true);
    }
    
}
