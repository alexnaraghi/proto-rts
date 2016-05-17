using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class RtsNetworkPlayer : NetworkBehaviour, IPlayer
{
    [SyncVar]
    public Color Color;

    // TODO: Generate synchronized seed.
    public int RandomnessSeed = 0;

    public NetCommandList CommandList = new NetCommandList();

    // Use this for initialization
    void Start () 
	{
        Injector.Get<CommandManager>().RegisterPlayer(this);
		
		if(isLocalPlayer && playerControllerId == 0)
		{
            RtsRandom.Seed = RandomnessSeed;
        }
    }
	
	void OnDestroy()
	{
        Injector.Get<CommandManager>().UnregisterPlayer(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isLocalPlayer && playerControllerId == 0)
		{
            var localCommands = Injector.Get<CommandManager>().LocalCommands;
			
			while(localCommands.Count > 0)
			{
                var command = localCommands.Dequeue();
                SendCommand(command);
            }
        }
	}
	
    // Removes and returns the commands for this lockstep.
	public Command[] RemoveCommandsForLockstep(int lockStep)
	{
        var commands = new List<Command>();

        var current = CommandList.First;

        while(current != null)
		{
            var next = current.Next;

            if(current.Value.LockStep == lockStep)
			{
                commands.Add(current.Value.Command);
                CommandList.Remove(current);
            }

            current = next;
        }

        return commands.ToArray();
    }

	public bool IsReadyForLockstep(int lockStep)
	{
        // TODO: Cache this result so we don't have to generate this twice.
        return RemoveCommandsForLockstep(lockStep).Length > 0;
    }
	
	public void SendCommand(Command command)
	{
		// We should send commands for a future lockstep, to ensure they are received.
        var lockstep = Injector.Get<CommandManager>().LockStep + 2;
        var teamNumber = command.TeamNumber;
		
        //Sexy serialization duck-typing
        if(command is SelectCommand)
		{
            var selectCommand = command as SelectCommand;

            var unitIds = selectCommand.SelectionsAsIds;
            CmdSelectUnits(lockstep, teamNumber, unitIds);
        }
		else if(command is TargetPositionCommand)
		{
            var targetPositionCommand = command as TargetPositionCommand;

            var position = targetPositionCommand.Destination;
            var attackers = targetPositionCommand.AttackersAsIds;
            var destination = targetPositionCommand.Destination;
            var isChaining = targetPositionCommand.IsChaining;

            CmdTargetPosition(lockstep, teamNumber, attackers, destination, isChaining);
        }
		else if(command is TargetRtsObjectCommand)
		{
            var targetRtsObjectCommand = command as TargetRtsObjectCommand;
    		
            var attackers = targetRtsObjectCommand.AttackersAsIds;
			var targetId = targetRtsObjectCommand.Target.Id;
            var isChaining = targetRtsObjectCommand.IsChaining;

            CmdTargetRtsObject(lockstep, teamNumber, attackers, targetId, isChaining);
        }
		else if(command is UnselectAllCommand)
		{
            var unselectAllCommand = command as UnselectAllCommand;

            CmdUnselectAll(lockstep, teamNumber);
        }
		else
		{
            Debug.LogError("Unsuported command: " + command.GetType().Name);
        }
    }


#region Client to Server
	[Command]
	public void CmdSelectUnits(int lockStep, int teamNumber, int[] unitIds)
	{
        RpcReceiveUnitsSelected(lockStep, teamNumber, unitIds);
    }
	
	[Command]
	public void CmdTargetPosition(int lockStep, int teamNumber, 
		int[] attackerIds, Vector3 destination, bool isChaining)
	{
        RpcReceiveTargetPosition(lockStep, teamNumber, attackerIds, destination, isChaining);
    }
  
    [Command]
    public void CmdTargetRtsObject(int lockStep, int teamNumber, 
		int[] attackerIds, int targetId, bool isChaining)
    {
        RpcReceiveTargetRtsObject(lockStep, teamNumber, attackerIds, targetId, isChaining);
    }
	
	[Command]
	public void CmdUnselectAll(int lockStep, int teamNumber)
	{
        RpcReceiveUnselectAll(lockStep, teamNumber);
    }
#endregion





#region Server to Client
    [ClientRpc]
	public void RpcReceiveUnitsSelected(int lockStep, int teamNumber, int[] unitIds)
	{
        var units = Injector.Get<GameState>().FromIds(unitIds);
        var cmd = new NetUnitCommand(lockStep, new SelectCommand(teamNumber, units));
        CommandList.AddLast(cmd);
	}

	[ClientRpc]
	public void RpcReceiveTargetPosition(int lockStep, int teamNumber, 
		int[] attackerIds, Vector3 destination, bool isChaining)
	{
        var attackers = Injector.Get<GameState>().FromIds(attackerIds).Select(u => u as Unit).ToArray();
        var cmd = new NetUnitCommand(lockStep, new TargetPositionCommand(teamNumber, attackers, destination, isChaining));
        CommandList.AddLast(cmd);
	}
	
	[ClientRpc]
	public void RpcReceiveTargetRtsObject(int lockStep, int teamNumber, 
		int[] attackerIds, int targetId, bool isChaining)
	{
        var gs = Injector.Get<GameState>();
        var attackers = gs.FromIds(attackerIds).Select(u => u as Unit).ToArray();
        RtsObject target;

        if(gs.RtsObjects.ContainsKey(targetId))
		{
        	target = Injector.Get<GameState>().RtsObjects[targetId];
		}
        else
		{
			// -----------------  EARLY OUT ---------------//
            Debug.LogWarning("No target of id " + targetId);
            return;
        }
        var cmd = new NetUnitCommand(lockStep, 
			new TargetRtsObjectCommand(teamNumber, attackers, target, isChaining));

        CommandList.AddLast(cmd);
	}
	
	
	[ClientRpc]
	public void RpcReceiveUnselectAll(int lockStep, int teamNumber) 
	{
        var cmd = new NetUnitCommand(lockStep, 
			new UnselectAllCommand(teamNumber));
        
        CommandList.AddLast(cmd);
	}
#endregion
}