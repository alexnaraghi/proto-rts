using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Assertions;

public class RtsNetworkPlayer : NetworkBehaviour, IPlayer
{
    [SyncVar]
    public int Id;
    public Color Color;

    // TODO: Generate synchronized seed.
    public int RandomnessSeed = 0;

    public Action<NetUnitCommand> CommandAddedEvent = delegate { };

    private Command _pendingLocalCommand;
    private int _pendingCommandLockStep = 0;
    public List<NetUnitCommand> CommandBuffer;
    public List<Confirmation> ConfirmationBuffer;
    
    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
    }

    // Use this for initialization
    void Awake()
    {
        CommandBuffer = new List<NetUnitCommand>();
        ConfirmationBuffer = new List<Confirmation>();
        var cmdManager = Injector.Get<CommandManager>();
        cmdManager.RegisterPlayer(this);
        
        // Buffer up fake packets for the initial locksteps before the send frame delay.
        for(int i = 0; i < cmdManager.SendLockstepDelay; i++)
        {
            CommandBuffer.Add(new NetUnitCommand(i, new NoOpCommand(1)));
            ConfirmationBuffer.Add(new Confirmation(i));
        }
        
        // Set the lockstep to the one before the first to send, so that when
        // we increment to the first lockstep, it will be correct.
        _pendingCommandLockStep = cmdManager.SendLockstepDelay - 1;
    }

    void OnDestroy()
    {
        Injector.Get<CommandManager>().UnregisterPlayer(this);
    }

    // Hook that it is time for the player to send commands.  Since there is a dependency on other
    // players being ready to move to the next send command, we get a list of IPlayers that we can
    // check for readiness.
    public void SendCommands(int lockStep, List<IPlayer> players)
    {
        // Only the local player sends commands.
        if (isLocalPlayer)
        {
            var cmdManager = Injector.Get<CommandManager>();
            var localCommands = cmdManager.LocalCommands;

            // Figure out how many players have already received and confirmed
            // this lockstep.
            int numReadyPlayers = 0;
            for(int i = 0; i < players.Count; i++)
            {
                if(players[i].IsReadyForLockstep(_pendingCommandLockStep))
                {
                    numReadyPlayers++;
                }
            }

            // Get the next command if it's the initial lockstep, if all players already
            // received the last command, and don't send commands for locksteps that haven't
            // occurred yet.
            // TODO: There seems to be a bug where executing multiple local actions in the 
            // same lockstep desyncs the simulation, investigate this.
            if(_pendingLocalCommand == null || 
                (numReadyPlayers == players.Count 
                && _pendingCommandLockStep != lockStep + cmdManager.SendLockstepDelay))
            {
                if(localCommands.Count > 0)
                {
                    _pendingLocalCommand = localCommands.Dequeue();
                }
                else
                {
                    _pendingLocalCommand = new NoOpCommand(1);
                }

                _pendingCommandLockStep ++;
            }
            
            // TODO: Optimize this to not send commands that are already confirmed (It seems to do this
            // when the remote clients are completely up to date on their packets).
            sendCommand(_pendingLocalCommand, _pendingCommandLockStep);
        }
    }
    
    // Removes and returns the commands for this lockstep.
    // TODO: Remove old commands
    public Command ProcessLockstep(int lockStep)
    {
        Command command = null;
        for (int i = 0; i < CommandBuffer.Count; i++)
        {
            if (CommandBuffer[i].LockStep == lockStep)
            {
                command = CommandBuffer[i].Command;
                //CommandBuffer.RemoveAt(i);
            }
        }

        for (int i = 0; i < ConfirmationBuffer.Count; i++)
        {
            if (ConfirmationBuffer[i].LockStep == lockStep)
            {
                //ConfirmationBuffer.RemoveAt(i);
                //i--;
            }
        }

        return command;
    }

    // Checks if we are ready for the next lockstep.  We are ready if we've received the command
    // for this lockstep and gotten confirmation that our command was received.
    public bool IsReadyForLockstep(int lockStep)
    {
        bool isPendingFull = false;
        bool isConfirmedFull = false;

        for (int i = 0; i < CommandBuffer.Count; i++)
        {
            if (CommandBuffer[i].LockStep == lockStep)
            {
                isPendingFull = true;
                break;
            }
        }

        if (isPendingFull)
        {
            for (int i = 0; i < ConfirmationBuffer.Count; i++)
            {
                if (ConfirmationBuffer[i].LockStep == lockStep)
                {
                    isConfirmedFull = true;
                    break;
                }
            }
        }
        /*
        Debug.Log(string.Format("Pending = {0}, Confirmed = {1}", 
            PendingFlag, isConfirmedFull));
            */
        return isPendingFull && isConfirmedFull;
    }

    // Flags that the local player has received the command of the given player id.
    public void Confirm(int lockStep, int playerId)
    {
        Assert.IsTrue(isLocalPlayer);

        CmdConfirmation(lockStep, playerId, Id);
        // Debug.Log(string.Format("L{0} Confirming sender{1}, receiver{2}", lockStep, playerId, Id));
    }

    // Sends a command to all clients.
    private void sendCommand(Command command, int lockstep)
    {
        //Only the local player sends commands.
        Assert.IsTrue(isLocalPlayer);
        
        // EARLY OUT if the command is invalid
        if (command == null)
        {
            Debug.LogError("Empty command");
            return;
        }

        var cmdManager = Injector.Get<CommandManager>();

        var teamNumber = command.TeamNumber;

        //Sexy serialization duck-typing
        if (command is NoOpCommand)
        {
            CmdNoOp(lockstep, teamNumber, Id);
        }
        else if (command is SelectCommand)
        {
            var selectCommand = command as SelectCommand;

            var unitIds = selectCommand.SelectionsAsIds;
            CmdSelectUnits(lockstep, teamNumber, Id, unitIds);
        }
        else if (command is TargetPositionCommand)
        {
            var targetPositionCommand = command as TargetPositionCommand;

            var position = targetPositionCommand.Destination;
            var attackers = targetPositionCommand.AttackersAsIds;
            var destination = targetPositionCommand.Destination;
            var isChaining = targetPositionCommand.IsChaining;

            CmdTargetPosition(lockstep, teamNumber, Id, attackers, destination, isChaining);
        }
        else if (command is TargetRtsObjectCommand)
        {
            var targetRtsObjectCommand = command as TargetRtsObjectCommand;

            var attackers = targetRtsObjectCommand.AttackersAsIds;
            var targetId = targetRtsObjectCommand.Target.Id;
            var isChaining = targetRtsObjectCommand.IsChaining;

            CmdTargetRtsObject(lockstep, teamNumber, Id, attackers, targetId, isChaining);
        }
        else if (command is UnselectAllCommand)
        {
            var unselectAllCommand = command as UnselectAllCommand;

            CmdUnselectAll(lockstep, teamNumber, Id);
        }
        else
        {
            Debug.LogError("Unsuported command: " + command.GetType().Name);
        }
    }
    
    // Flags that a confirmation packet was received for the given lockstep.  Discards duplicates.
    private void confirmUnique(int lockStep)
    {
        bool alreadyConfirmed = false;
        for(int i = 0; i < ConfirmationBuffer.Count; i++)
        {
            if(ConfirmationBuffer[i].LockStep == lockStep)
            {
                alreadyConfirmed = true;
            }
        }
        
        if(!alreadyConfirmed)
        {
            ConfirmationBuffer.Add(new Confirmation(lockStep));
        }
    }
    
    // Adds a command to play for the given lockstep.  Discards duplicates.
    private void addCommandUnique(NetUnitCommand command)
    {
        bool alreadyBuffered = false;
        for(int i = 0; i < CommandBuffer.Count; i++)
        {
            if(CommandBuffer[i].LockStep == command.LockStep)
            {
                alreadyBuffered = true;
            }
        }
        
        if(!alreadyBuffered)
        {
            CommandBuffer.Add(command);
        }
    }

    // This is what is invoked on clients when they receive either a local or remote command.
    // Process the command and send confirmation that it was received.
    private void addCommand(NetUnitCommand command, int playerId)
    {
        Assert.IsTrue(playerId == Id);

        CommandAddedEvent(command);
        addCommandUnique(command);

        /*
        Debug.Log(string.Format("Received {0} command at lockstep {1}, type {2}\nplayer {3}",
            isLocalPlayer ? "Local" : "Remote", command.LockStep, command.Command.GetType(),
            playerId));
            */
        if (isLocalPlayer)
        {
            /*
            Debug.Log(string.Format("L{0} Confirmed local player{1}", lockStep, playerId));
            */
            confirmUnique(command.LockStep);
        }
        else
        {
            // This is an oddity with the current implementation.  Our remote client would like to send
            // confirmation that it received a packet, but only local UNET clients with authority can 
            // send packets.  So we have to send confirmation through our local player.
            // The command manager is the only service at the moment that "knows" about all the players,
            // so we route through it to get that player and send confirmation.
            var localPlayer = Injector.Get<CommandManager>().GetLocalPlayer();

            Assert.IsNotNull(localPlayer);
            
            localPlayer.Confirm(command.LockStep, playerId);
        }
    }

    #region Client to Server
    [Command]
    public void CmdSelectUnits(int lockStep, int teamNumber, int playerId, int[] unitIds)
    {
        RpcReceiveUnitsSelected(lockStep, teamNumber, playerId, unitIds);
    }

    [Command]
    public void CmdTargetPosition(int lockStep, int teamNumber, int playerId,
        int[] attackerIds, Vector3 destination, bool isChaining)
    {
        RpcReceiveTargetPosition(lockStep, teamNumber, playerId, attackerIds, destination, isChaining);
    }

    [Command]
    public void CmdTargetRtsObject(int lockStep, int teamNumber, int playerId,
        int[] attackerIds, int targetId, bool isChaining)
    {
        RpcReceiveTargetRtsObject(lockStep, teamNumber, playerId, attackerIds, targetId, isChaining);
    }

    [Command]
    public void CmdUnselectAll(int lockStep, int teamNumber, int playerId)
    {
        RpcReceiveUnselectAll(lockStep, teamNumber, playerId);
    }

    [Command]
    public void CmdNoOp(int lockStep, int teamNumber, int playerId)
    {
        RpcReceiveNoOp(lockStep, teamNumber, playerId);
    }


    [Command]
    public void CmdConfirmation(int lockStep, int senderId, int receiverId)
    {
        RpcReceiveConfirmation(lockStep, senderId, receiverId);
    }
    #endregion

    #region Server to Client
    [ClientRpc]
    public void RpcReceiveUnitsSelected(int lockStep, int teamNumber, int playerId, int[] unitIds)
    {
        var units = Injector.Get<GameState>().FromIds(unitIds);
        var cmd = new NetUnitCommand(lockStep, new SelectCommand(teamNumber, units));
        addCommand(cmd, playerId);
    }

    [ClientRpc]
    public void RpcReceiveTargetPosition(int lockStep, int teamNumber, int playerId,
        int[] attackerIds, Vector3 destination, bool isChaining)
    {
        var attackers = Injector.Get<GameState>().FromIds(attackerIds).Select(u => u as Unit).ToArray();
        var cmd = new NetUnitCommand(lockStep, new TargetPositionCommand(teamNumber, attackers, destination, isChaining));
        addCommand(cmd, playerId);
    }

    [ClientRpc]
    public void RpcReceiveTargetRtsObject(int lockStep, int teamNumber, int playerId,
        int[] attackerIds, int targetId, bool isChaining)
    {
        var gs = Injector.Get<GameState>();
        var attackers = gs.FromIds(attackerIds).Select(u => u as Unit).ToArray();
        RtsObject target;

        if (gs.RtsObjects.ContainsKey(targetId))
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

        addCommand(cmd, playerId);
    }


    [ClientRpc]
    public void RpcReceiveUnselectAll(int lockStep, int teamNumber, int playerId)
    {
        var cmd = new NetUnitCommand(lockStep,
            new UnselectAllCommand(teamNumber));

        addCommand(cmd, playerId);
    }

    [ClientRpc]
    public void RpcReceiveNoOp(int lockStep, int teamNumber, int playerId)
    {
        addCommand(new NetUnitCommand(lockStep, new NoOpCommand(teamNumber)), playerId);
    }


    [ClientRpc]
    public void RpcReceiveConfirmation(int lockStep, int senderId, int receiverId)
    {
        // Debug.Log(string.Format("CONFIRM REMOTE player sender {0}, receiver {1}", senderId, receiverId));
        // Local players automatically confirm their own packets, so we only care about remotes here.
        if (receiverId == Id && !isLocalPlayer)
        {
            confirmUnique(lockStep);
        }
    }
    #endregion
    
}