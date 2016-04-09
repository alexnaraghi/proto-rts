using UnityEngine;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour 
{
	public Queue<Command> CommandHistory = new Queue<Command>();
    
    public void QueueCommand(Command command)
    {
        Debug.Log("Command Queued: " + command.GetType().Name);
        
        CommandHistory.Enqueue(command);
        
        //For now, just immediate execute a command.
        command.Execute();
    }
}
