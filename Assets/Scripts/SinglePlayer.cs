using UnityEngine;
using System.Collections.Generic;
using System;

public class SinglePlayer : MonoBehaviour, IPlayer
{
    List<Command> _commands = new List<Command>();
    public Command[] RemoveCommandsForLockstep(int lockStep)
    {
        var allCommands = _commands.ToArray();
        _commands.Clear();
        return allCommands;
    }

    public bool IsReadyForLockstep(int lockStep)
    {
        return true;
    }

    void Awake () 
	{
		if(GameObject.FindObjectOfType(typeof(RtsNetworkPlayer)) != null)
		{
            this.enabled = false;
        }
        else
        {
            Injector.Get<CommandManager>().RegisterPlayer(this);
        }
	}
    
    void Update()
    {
        var commandManager = Injector.Get<CommandManager>();
        
        var localCommands = Injector.Get<CommandManager>().LocalCommands;
			
        while(localCommands.Count > 0)
        {
            var command = localCommands.Dequeue();
            _commands.Add(command);
        }
    }
}
