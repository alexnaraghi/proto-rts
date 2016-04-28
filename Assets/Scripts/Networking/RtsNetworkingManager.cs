using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RtsNetworkingManager : NetworkLobbyManager
{
    
	// ==================== SERVER ====================
	

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayersForConnection(conn);
        Debug.Log("OnServerDisconnect");
    }
    
    // called when a client is ready
    public override void OnServerReady(NetworkConnection conn)
    {
        NetworkServer.SetClientReady(conn);
        Debug.Log("OnServerReady");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer");
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
		
        Debug.Log("OnServerRemovePlayer");
    }

    // ==================== CLIENT ====================

    public override void OnStartClient(NetworkClient lobbyClient)
    {
		
        Debug.Log("OnStartClient");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("OnClientConnect");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Debug.Log("OnClientDisconnect");
    }

    public override void OnStopClient()
    {
        Debug.Log("OnStopClient");
    }
	
	public override void OnClientError(NetworkConnection conn, int errorCode)
	{
        Debug.Log("OnClientError: " + errorCode);
    }

    // ------------------------ lobby server virtuals ------------------------

    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        Debug.Log("OnLobbyServerConnect");
    }

    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnLobbyServerDisconnect");
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        return null;
    }

    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("OnLobbyServerPlayersReady");
		
    }

    // ------------------------ lobby client virtuals ------------------------

    public override void OnLobbyClientEnter()
    {
        Debug.Log("OnLobbyClientEnter");		
    }

    public override void OnLobbyClientExit()
    {
        Debug.Log("OnLobbyClientExit");
    }
}
