using System.Collections.Generic;

// Represents a player that can create and process commands for locksteps.
public interface IPlayer
{
    bool IsReadyForLockstep(int lockStep);
    void SendCommands(int lockStep, List<IPlayer> players);
    Command ProcessLockstep(int lockStep);
    bool IsLocalPlayer { get; }
    void Confirm(int lockStep, int playerId);
}