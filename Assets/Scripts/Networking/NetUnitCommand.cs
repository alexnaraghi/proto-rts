using System.Collections.Generic;

public struct NetUnitCommand
{
	public Command Command;
	public int LockStep;

	public NetUnitCommand(int lockStep, Command command)
	{
        Command = command;
        LockStep = lockStep;
    }
}

public struct Confirmation
{
    public int LockStep;

    public Confirmation(int lockStep)
	{
        LockStep = lockStep;
    }
}

public class NetCommandList : LinkedList<NetUnitCommand>{ }