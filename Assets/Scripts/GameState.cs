using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class GameState : MonoBehaviour
{
    public Dictionary<int, RtsObject> RtsObjects;
    
    public HashSet<int> SelectedObjectIds;
    
    public List<RtsObject> ObjectsToDestroy;
    
    public int LocalTeam;

    // The map's bounding rectangle in units, represented as the half-width,length,height
    public Vector3 MapBounds = new Vector3(150, 80, 80);

    void Awake()
    {
        ObjectsToDestroy = new List<RtsObject>();
        SelectedObjectIds = new HashSet<int>();
        RtsObjects = new Dictionary<int, RtsObject>();
        
        //TODO: Team selection or something
        LocalTeam = 1;
        
        Populate();
    }
    
    void Update()
    {
        for(int i = 0; i < ObjectsToDestroy.Count; i++)
        {
            Destroy(ObjectsToDestroy[i].gameObject);
        }
        ObjectsToDestroy.Clear();
    }
    
    //Temporary, until we have level files
    private void Populate()
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Selectable"))
        {
            var unit = obj.GetComponent<Unit>();
            unit.Id = Injector.Get<GameObjectFactory>()._idIndex++;
            RtsObjects.Add(unit.Id, unit);
        }
        
        foreach(var obj in GameObject.FindGameObjectsWithTag("Base"))
        {
            var baseObj = obj.GetComponent<RtsObject>();
            baseObj.Id = Injector.Get<GameObjectFactory>()._idIndex++;
            RtsObjects.Add(baseObj.Id, baseObj);
        }
        
        foreach(var obj in GameObject.FindGameObjectsWithTag("Mine"))
        {
            var mine = obj.GetComponent<RtsObject>();
            mine.Id = Injector.Get<GameObjectFactory>()._idIndex++;
            RtsObjects.Add(mine.Id, mine);
        }
    }
    
    public void Kill(RtsObject rtsObject)
    {
        Assert.IsNotNull(rtsObject);
        
        var id = rtsObject.Id;
        if(RtsObjects.ContainsKey(id))
        {
            var foundUnit = RtsObjects[id];
            SelectedObjectIds.Remove(id);
            RtsObjects.Remove(id);
            foundUnit.IsAlive = false;
            ObjectsToDestroy.Add(foundUnit);
        }
    }
}
