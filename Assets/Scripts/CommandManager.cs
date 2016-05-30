using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Command manager is the simulation authority.  It manages lockstep synchronization between player
// commands and the game simulation.
public class CommandManager : MonoBehaviour
{
    public Queue<Command> LocalCommands = new Queue<Command>();
    public List<string> DebugCommandHistory = new List<string>();

    private List<IPlayer> _players = new List<IPlayer>();
    private float _accumulatedTime;
    private float _frameSeconds = 0.033333f;

    private int _gameFramesPerLockStepTurn = 6;

    public int SendFrameDelay = 3;
    private int _frame = 0;

    public int LockStep
    {
        get;
        private set;
    }

    // This allows local input to queue a command.  IPlayers can process it as they wish by immediately
    // passing it back, by sending it over the network, etc.
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
    
    // This is the main update loop of the game simulation.
    public void Update()
    {
        _accumulatedTime += Time.deltaTime;

        while (_accumulatedTime > _frameSeconds)
        {
            GameFrameTurn();
            _accumulatedTime -= _frameSeconds;
        }
    }

    // A game frame turn executes a single step of the simulation.
    private void GameFrameTurn()
    {
        bool isWaiting = false;
        
        // This is the first frame of the lockstep.
        if (_frame == 0)
        {
            // Each player should process their commands.
            foreach (var player in _players)
            {
                player.SendCommands(LockStep, _players);
            }

            // If players are ready for this lockstep, let them execute their commands.  Otherwise, wait.
            if (IsLockStepReady())
            {
                ExecuteCommands();
                LockStep++;
            }
            else
            {
                isWaiting = true;
            }
        }
        
        // Wait for lockstep to be processed before continuing
        if (!isWaiting)
        {
            // We can't use an enumerable here because new units can be created during the 
            // foreach.
            // TODO: Optimize using a cached array.
            var rtsObjects = Injector.Get<GameState>().RtsObjects.Values.ToArray();

            //update the game
            foreach (var obj in rtsObjects)
            {
                obj.GameUpdate(_frameSeconds);
            }

            // There are some limited actions that must occur after the updates are all processed.
            foreach (var obj in rtsObjects)
            {
                obj.LateGameUpdate(_frameSeconds);
            }

            Injector.Get<GameState>().GameUpdate(_frameSeconds);

            _frame++;
            if (_frame == _gameFramesPerLockStepTurn)
            {
                _frame = 0;
            }
        }
    }
    
    private bool IsLockStepReady()
    {
//        int expectedPlayers = _players.Count;
        //TODO: Add this to lobby params
        int expectedPlayers = 2;
        int readyPlayers = 0;
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].IsReadyForLockstep(LockStep))
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
            var command = _players[i].ProcessLockstep(LockStep);
            if(command != null)
            {
                command.Execute();
            }
        }
    }
    
    public IPlayer GetLocalPlayer()
    {
        IPlayer localPlayer = null;
        
        foreach(var player in _players)
        {
            if(player.IsLocalPlayer)
            {
                localPlayer = player;
            }
        }

        return localPlayer;
    }
}
