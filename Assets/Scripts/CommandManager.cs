using UnityEngine;
using System.Collections.Generic;

public class CommandManager : MonoBehaviour 
{
	public Queue<Command> LocalCommands = new Queue<Command>();
    public List<string> DebugCommandHistory = new List<string>();

    private List<IPlayer> _players = new List<IPlayer>();
    private float _accumulatedTime;
    private float _frameSeconds = 0.033333f;

    private int _gameFramesPerLockStepTurn = 6;
    private float _gameFramesPerSecond = 20;
    private int _frame = 0;

    public int LockStep
    {
        get;
        private set;
    }

    public void QueueLocalCommand(Command command)
    {
        DebugCommandHistory.Add("Command Queued: " + command.GetType().Name);
        
        LocalCommands.Enqueue(command);
    }
     
    public void RegisterPlayer(IPlayer player)
    {
        _players.Add(player);
    }
    
    public void UnregisterPlayer(IPlayer player)
    {
        _players.Remove(player);
    }
    
    public void Update()
    {
        //Some implementation questions in here that we should resolve.
        
        
        // Maybe we should wait here until players are ready.  Also handle disconnections.
        // This is just the basic implementation for testing.
        _accumulatedTime += Time.deltaTime;
        
        while(_accumulatedTime > _frameSeconds)
        {
            GameFrameTurn();
            _accumulatedTime -= _frameSeconds;
        }
    }
    
    private void GameFrameTurn()
    {
        if(_frame == 0)
        {
            //Do lockstep turn
            if(IsLockStepReady())
            {
                //Why do we not update the game on the frame that the lockstep is ready?
                ExecuteCommands();
                LockStep++;
                _frame++;
            }
        }
        //else
        {
            var rtsObjects = Injector.Get<GameState>().RtsObjects.Values;
            
            //update the game
            foreach(var obj in rtsObjects)
            {
                obj.GameUpdate(_frameSeconds);
            }
            
            rtsObjects = Injector.Get<GameState>().RtsObjects.Values;
            
            foreach(var obj in rtsObjects)
            {
                obj.LateGameUpdate(_frameSeconds);
            }

            Injector.Get<GameState>().GameUpdate(_frameSeconds);

            _frame++;
            if(_frame == _gameFramesPerLockStepTurn)
            {
                _frame = 0;
            }
        }
    }
    
    private bool IsLockStepReady()
    {
        int expectedPlayers = _players.Count;
        int readyPlayers = 0;
        for(int i = 0; i < _players.Count; i++)
        {
            if( _players[i].IsReadyForLockstep(LockStep))
            {
                readyPlayers++;
            }
        }

        return expectedPlayers == readyPlayers;
    }
    
    private void ExecuteCommands()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            var commands = _players[i].GetCommandsForLockstep(LockStep);
            foreach(var command in commands)
            {
                command.Execute();
            }
        }
    }
}

public interface IPlayer
{
    bool IsReadyForLockstep(int lockStep);

    Command[] GetCommandsForLockstep(int lockStep);
}