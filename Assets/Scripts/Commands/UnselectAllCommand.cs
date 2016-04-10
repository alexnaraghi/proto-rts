using System.Collections.Generic;

[System.Serializable]
public class UnselectAllCommand : Command 
{
    public HashSet<int> PreviousSelections;
    
    public UnselectAllCommand(int teamNumber)
    {
        TeamNumber = teamNumber;
    }

	public override void Execute()
    {
        var gameState = Injector.Get<GameState>();
        
        PreviousSelections = gameState.SelectedObjectIds;
        
        InputManager.ForeachSelectedObject(gameState.SelectedObjectIds, (rtsObj) =>
        {
            rtsObj.MySelectableObject.ToggleSelection(false);
        });
        
        gameState.SelectedObjectIds.Clear();
    }
    
    public override void Undo()
    {
        var gameState = Injector.Get<GameState>();
        gameState.SelectedObjectIds = PreviousSelections;
    }
}
