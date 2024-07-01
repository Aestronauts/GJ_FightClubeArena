using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkingLobby : MonoBehaviour
{

    private Lobby hostLob;
    private float lobbyHeartbeatTimer = 25f;
    private int lobIdSelected = -1;
    QueryResponse queryLobResp;


    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed In {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();// can use this with steam to link accounts
    }

    public void LateUpdate()
    {
        KeepLobbyActive();
    }

    public async void CreateLobby(bool _isPrivate)
    {
        try
        {
            string lobName = "NewLobby";
            int lobMaxPlayers = 8;
            CreateLobbyOptions createLobOptions = new CreateLobbyOptions { IsPrivate = _isPrivate, };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobName, lobMaxPlayers, createLobOptions);
            hostLob = lobby;
            
            Debug.Log($"Created Lobby - {lobby.Name} - {lobby.MaxPlayers} possible player slots \nLobby Id - {lobby.Id} \nJoinCode - {lobby.LobbyCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void KeepLobbyActive()
    {
        if (hostLob != null)
        {
            lobbyHeartbeatTimer -= Time.deltaTime;

            if (lobbyHeartbeatTimer <= 0)
            {
                lobbyHeartbeatTimer = 25f;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLob.Id);
            }
        }
    }

    public async void JoinLobbyId(int _lobId)
    {
        lobIdSelected = _lobId;

        if (_lobId != -1 && queryLobResp != null)
            await Lobbies.Instance.JoinLobbyByIdAsync(queryLobResp.Results[_lobId].Id);
    }

    public async void JoinLobbyCode(string _lobJoinCode)
    {
        await Lobbies.Instance.JoinLobbyByCodeAsync(_lobJoinCode);
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobOptions = new QueryLobbiesOptions
            {
                Count = 20,
                Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT) },
                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created) }
            };
            queryLobResp = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log($"Lobbies found - {queryLobResp.Results.Count}");
            foreach (Lobby lob in queryLobResp.Results)
            {
                Debug.Log($"Lobby - {lob.Name} - {lob.MaxPlayers} possible player slots");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }
}
