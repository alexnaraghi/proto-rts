using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class RtsLobbyHook : LobbyHook
{
	public override void OnLobbyServerSceneLoadedForPlayer(
		NetworkManager manager, 
		GameObject lobbyPlayerObj, 
		GameObject gamePlayerObj) 
	{
        var player = gamePlayerObj.GetComponent<RtsNetworkPlayer>();
        var lobbyPlayer = lobbyPlayerObj.GetComponent<LobbyPlayer>();

        player.Color = lobbyPlayer.playerColor;
    }
}