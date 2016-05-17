using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public Dictionary<int, RtsObject> RtsObjects;
    
    public HashSet<int> SelectedObjectIds;
    
    public List<RtsObject> ObjectsToDestroy;
    
    public int LocalTeam;
    
    public int TeamCount
    {
        get;
        private set;
    }

    // The map's bounding rectangle in units, represented as the half-width,length,height
    public Vector3 MapBounds = new Vector3(150, 80, 80);

    void Awake()
    {
        ObjectsToDestroy = new List<RtsObject>();
        SelectedObjectIds = new HashSet<int>();
        RtsObjects = new Dictionary<int, RtsObject>();
        
        //TODO: Team selection or something
        LocalTeam = 1;
        TeamCount = 2;

        Populate();
    }
    
    public void GameUpdate(float deltaSeconds)
    {
        for(int i = 0; i < ObjectsToDestroy.Count; i++)
        {
            SelectedObjectIds.Remove(ObjectsToDestroy[i].Id);
            Destroy(ObjectsToDestroy[i].gameObject);
        }
        ObjectsToDestroy.Clear();

        var info = CheckVictoryConditions();
        if(info.IsOver)
        {
            //TODO: game over stuff
            LeanTween.delayedCall(1f, ()=> SceneManager.LoadScene("GameOver"));
        }
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
            RtsObjects.Remove(id);
            foundUnit.IsAlive = false;
            ObjectsToDestroy.Add(foundUnit);
        }
    }
    
    public RtsObject[] FromIds(int[] unitIds)
    {
        var units = new RtsObject[unitIds.Length];
        for(int i = 0; i < unitIds.Length; i++)
        {
            Assert.IsTrue(RtsObjects.ContainsKey(i));
            units[i] = RtsObjects[unitIds[i]];
        }
        return units;
    }
    
    private VictoryInfo CheckVictoryConditions()
    {
        VictoryInfo info = default(VictoryInfo);
        
        // See if all the bases are owned by a single team.
        int team = -1;
        bool isUnanimous = true;
        foreach(var o in RtsObjects.Values)
        {
            if (o is Base)
            {
                if (team == -1)
                {
                    team = o.Team;
                }
                else if(team != o.Team)
                {
                    isUnanimous = false;
                    break;
                }
            }
        }
        
        if(team > 0 && isUnanimous)
        {
            info = new VictoryInfo() { IsOver = true, Team = team };
        }

        return info;
    }
}

struct VictoryInfo
{
    public bool IsOver;
    public int Team;
}
