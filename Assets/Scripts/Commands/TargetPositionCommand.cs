using UnityEngine;
using System.Linq;

[System.Serializable]
public class TargetPositionCommand : Command
{
    public Unit[] Attackers;
    
    public Vector3 Destination;
    
    public bool IsChaining;
    
    public int[] AttackersAsIds
    {
        get
        {
            return Attackers.Select(u => u.Id).ToArray();
        }
    }
    
    public TargetPositionCommand(int teamNumber, Unit[] attackers, Vector3 destination, bool isChaining)
    {
        TeamNumber = teamNumber;
        Attackers = attackers;
        Destination = destination;
        IsChaining = isChaining;
    }

	public override void Execute()
    {
        Debug.LogFormat("Targeted position [{0}]", Destination.ToString());
        
        Formation.Form(Attackers, Destination, FormationType.Scattered, IsChaining);
    }
    
    public override void Undo()
    {
        //TODO: Undo
    }
}
