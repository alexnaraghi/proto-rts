using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : RtsObject
{
    public const float ORBIT_RADIUS = 25f;
    public Unit ProductionUnitPrefab;

    public Image FillImage;

    public List<Unit> OrbitingObjects;
    
    public int Resource;
    
    public bool isProducing;

    // For now, let's have production capped by unloading, not a separate production
    // track.
    const float UNIT_PRODUCTION_SECONDS = 0f;
    
    private float _productionSeconds;

    private Unit _unloadingUnit;

    private float _ownershipPercent;
    private const float CAPTURE_PERCENT_PER_UNIT_PER_SECOND = 1f;
    
    public enum BaseCaptureState
    {
        Owned,
        Contested,
        Gaining,
        Losing
    }

    protected override void Start()
    {
        base.Start();

        Assert.IsTrue(FillImage != null);
        Assert.IsTrue(ProductionUnitPrefab != null);
    }
    
    void Update()
    {
        if (!isProducing && Resource > 0)
        {
            Resource--;
            isProducing = true;
        }

        if (isProducing)
        {
            if (_productionSeconds >= UNIT_PRODUCTION_SECONDS)
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

        // Calculate capture rate.
        // Currently this only works for 2 team contests.
        {
            bool isOwned = Team != 0;
            bool hasOwnerUnit = false;
            bool hasEnemyUnit = false;

            // Determine if we are contested.
            for (int i = 0; i < OrbitingObjects.Count; i++)
            {
                var obj = OrbitingObjects[i];
                
                // Early out if not in range.
                //TODO: Clean out the list of null objects.
                if(obj == null || !IsInCaptureRange(obj))
                {
                    continue;
                }
                
                if(obj.Team != Team)
                {
                    hasEnemyUnit = true;
                }
                else
                {
                    hasOwnerUnit = true;
                }
                
                if(hasOwnerUnit && hasEnemyUnit)
                {
                    break;
                }
            }

            bool isContested = hasOwnerUnit && hasEnemyUnit;
            bool isCompletelyBelongingToTeam = isOwned 
                && !isContested 
                && _ownershipPercent >= 1.0f - float.Epsilon;
            
            bool wasCaptured = false;

            if (!isContested && !isCompletelyBelongingToTeam)
            {
                for (int i = 0; i < OrbitingObjects.Count; i++)
                {
                    var obj = OrbitingObjects[i];
                    
                    // Early out if not in range.
                    if(obj == null || !IsInCaptureRange(obj))
                    {
                        continue;
                    }
                    
                    // All the units will be the same team since we've already determined we are 
                    // uncontested.
                    if(!isOwned || hasOwnerUnit)
                    {
                        _ownershipPercent += CAPTURE_PERCENT_PER_UNIT_PER_SECOND * 0.01f * Time.deltaTime;
                    }
                    else
                    {
                        _ownershipPercent += CAPTURE_PERCENT_PER_UNIT_PER_SECOND * 0.01f * Time.deltaTime;                        
                    }
                    
                    if(_ownershipPercent <= 0f)
                    {
                        isOwned = false;
                        _ownershipPercent = 0f;
                    }
                    else if(!isOwned && _ownershipPercent >= 1f)
                    {
                        isOwned = true;
                        _ownershipPercent = 1f;
                        this.Team = obj.Team;
                        wasCaptured = true;
                        break;
                    }
                }
                
                SetFill(_ownershipPercent);
                if(wasCaptured)
                {
                    AnimateSuccessfulCapture();
                }
            }
        }
    }
    
    private bool IsInCaptureRange(Unit unit)
    {
        return (unit.transform.position - transform.position).sqrMagnitude 
            <= (ORBIT_RADIUS * ORBIT_RADIUS) / 2;
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
    private void CompleteUnit()
    {
        var unit = Injector.Get<GameObjectFactory>().CreateRtsObject(ProductionUnitPrefab);
        
        //TODO: use the right team
        unit.Team = 1;
        unit.transform.position = transform.position;
        unit.PushState(new OrbitingState(this, ORBIT_RADIUS), true);
    }
    
    
    public bool IsAvailable
    {
        get {
            return _unloadingUnit == null;
        }
    }
    
    public void BeginUnloading(Unit unit)
    {
        Assert.IsNull(_unloadingUnit);
        _unloadingUnit = unit;
    }
    
    public void EndUnloading(Unit unit)
    {
        Assert.IsNotNull(_unloadingUnit);
        if (unit == _unloadingUnit)
        {
            _unloadingUnit = null;
        }
        else
        {
            Debug.LogWarning("Unit declared that it ended unloading, but wasn't unloading...");
        }
    }
    
    private void SetFill(float ratio)
    {
        FillImage.color = Color.white;
        FillImage.fillAmount = ratio;
    }
    
    private void AnimateSuccessfulCapture()
    {
        LeanTween.cancel(FillImage.gameObject);
        LeanTween.delayedCall(FillImage.gameObject, 1f, () => {
            FillImage.color = Color.clear;
            SetTeamColor(Team);
        });
    }
}
