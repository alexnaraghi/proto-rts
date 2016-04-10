using UnityEngine;

[System.Serializable]
public class TargetRtsObjectCommand : Command 
{
    public Unit[] Attackers;
    
    public RtsObject Target;
    
    public bool IsChaining;
    
    public TargetRtsObjectCommand(int teamNumber, Unit[] attackers, RtsObject target, bool isChaining)
    {
        TeamNumber = teamNumber;
        Attackers = attackers;
        Target = target;
        IsChaining = isChaining;
    }

	public override void Execute()
    {
        Debug.LogFormat("Targeted [{0} ({1},{2})]", Target.ToString(), 
            Target.transform.position.x, 
            Target.transform.position.z);
            
        for(int i = 0; i < Attackers.Length; i++)
        {
            var attacker = Attackers[i];
            
            Target.OnTargeted(attacker, IsChaining);
        }
    }
    
    public override void Undo()
    {
        //TODO: Undo
    }
}

