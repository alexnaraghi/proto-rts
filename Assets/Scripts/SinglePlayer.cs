using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

public class SinglePlayer : MonoBehaviour, IPlayer
{
    List<Command> _commands = new List<Command>();

    public bool IsLocalPlayer
    {
        get
        {
            return false;
        }
    }

    public Command ProcessLockstep(int lockStep)
    {
        Assert.IsTrue(_commands.Count < 2);
        var command = _commands.Count > 0 ? _commands[0] : null;
        _commands.Clear();
        return command;
    }
    
    public bool IsReadyForLockstep(int lockStep)
    {
        // Single player is always ready for a lockstep since we aren't waiting for
        // commands to come in.
        return _commands.Count > 0;
    }

    void Start()
    {
        if (GameObject.FindObjectOfType(typeof(RtsNetworkPlayer)) != null)
        {
            this.enabled = false;
        }
        else
        {
            Injector.Get<CommandManager>().RegisterPlayer(this);
        }
    }
    
    public void SendCommands(int lockStep, List<IPlayer> players)
    {
        
    }


    public void OnLockStepStarted(int lockStep)
    {
        var commandManager = Injector.Get<CommandManager>();

        var localCommands = Injector.Get<CommandManager>().LocalCommands;

        if (localCommands.Count == 0)
        {
            _commands.Add(new NoOpCommand(1));
        }
        else
        {
            while (localCommands.Count > 0)
            {
                var command = localCommands.Dequeue();
                _commands.Add(command);
            }
        }
    }

    public void Confirm(int lockStep, int playerId)
    {
        throw new NotImplementedException();
    }
}