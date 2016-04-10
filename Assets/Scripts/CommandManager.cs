using UnityEngine;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour 
{
	public Queue<Command> CommandHistory = new Queue<Command>();
    public List<string> DebugCommandHistory = new List<string>();

    public void QueueCommand(Command command)
    {
        DebugCommandHistory.Add("Command Queued: " + command.GetType().Name);
        
        CommandHistory.Enqueue(command);
        
        //For now, just immediate execute a command.
        command.Execute();
    }
}
