using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;
using System.Linq;

[System.Serializable]
public class SelectCommand : Command 
{
    public HashSet<int> PreviousSelections;
    public RtsObject[] Selections;
    
    public int[] SelectionsAsIds
    {
        get
        {
            return Selections.Select(u => u.Id).ToArray();
        }
    }
    
    public SelectCommand(int teamNumber, RtsObject[] selections)
    {
        TeamNumber = teamNumber;
        Selections = selections; 
    }

	public override void Execute()
    {
        var gameState = Injector.Get<GameState>();
        PreviousSelections = gameState.SelectedObjectIds;
        
        foreach(var selection in Selections)
        {
            if (IsSelectable(selection))
            {
                gameState.SelectedObjectIds.Add(selection.Id);
                selection.MySelectableObject.ToggleSelection(true);
            }
        }
        
        //Just for debugging
        // int debugCount = 0;
        // StringBuilder b = new StringBuilder();
        // foreach(var selection in Selections)
        // {
        //     if(IsSelectable(selection))
        //     {
        //         debugCount ++;
        //         b.AppendFormat("[{0} ({1},{2})]", selection.ToString(), 
        //             selection.transform.position.x, 
        //             selection.transform.position.z);
        //     }
        // }
        // if(debugCount > 0)
        // {
        //     b.Insert(0, "Selected (" + debugCount + "): ");
        //     Debug.Log(b.ToString());
        // }
    }
    
    private bool IsSelectable(RtsObject selection)
    {
        var gameState = Injector.Get<GameState>();
        return (selection.Team == gameState.LocalTeam && selection.IsSelectable);
    }
    
    public override void Undo()
    {
        var gameState = Injector.Get<GameState>();
        gameState.SelectedObjectIds = PreviousSelections;
    }
}
