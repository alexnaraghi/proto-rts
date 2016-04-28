using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System;

public class RtsMatchmaker : MonoBehaviour
{
    List<MatchDesc> matchList = new List<MatchDesc>();

    bool matchCreated;
    NetworkMatch networkMatch;

    public Action OnMatchFound = delegate { };
    public Action OnMatchFailed = delegate { };

    void Awake()
    {
        networkMatch = gameObject.AddComponent<NetworkMatch>();

        var appId = (UnityEngine.Networking.Types.AppID)Config.AppId;
        networkMatch.SetProgramAppID(appId);
    }

    public void Matchmake()
    {
        networkMatch.ListMatches(0, 20, "", OnMatchList);
    }

    private void CreateMatchRequest()
    {
        CreateMatchRequest create = new CreateMatchRequest();
        create.name = "NewRoom";
        create.size = 2;
        create.advertise = true;
        create.password = "";

        networkMatch.CreateMatch(create, OnMatchCreate);
    }

    private void OnMatchCreate(CreateMatchResponse matchResponse)
    {
        if (matchResponse.success)
        {
            OnMatchFound();
            Debug.Log("Create match succeeded");
            matchCreated = true;
            Utility.SetAccessTokenForNetwork(matchResponse.networkId, 
                new NetworkAccessToken(matchResponse.accessTokenString));
                
            NetworkServer.Listen(new MatchInfo(matchResponse), 9000);
        }
        else
        {
            OnMatchFailed();
            Debug.LogError("Create match failed");
        }
    }

    private void OnMatchList(ListMatchResponse matchListResponse)
    {
        if (matchListResponse.success)
        {
            if (matchListResponse.matches != null && matchListResponse.matches.Count > 0)
            {
                networkMatch.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
            }
            else
            {
                CreateMatchRequest();
            }
        }
        else
        {
            Matchmake();
        }
    }

    private void OnMatchJoined(JoinMatchResponse matchJoin)
    {
        if (matchJoin.success)
        {
            OnMatchFound();
            Debug.Log("Join match succeeded");
            if (matchCreated)
            {
                Debug.LogWarning("Match already set up, aborting...");
                return;
            }
            Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
            NetworkClient myClient = new NetworkClient();
            myClient.RegisterHandler(MsgType.Connect, OnConnected);
            myClient.Connect(new MatchInfo(matchJoin));
        }
        else
        {
            OnMatchFailed();
            Debug.LogError("Join match failed");
        }
    }

    private void OnConnected(NetworkMessage msg)
    {
        Debug.Log("Connected!");
    }
}