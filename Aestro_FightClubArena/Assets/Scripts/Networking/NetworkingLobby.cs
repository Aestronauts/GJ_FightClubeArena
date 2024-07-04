using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
//------- ^ required for unity lobby services
using System.Collections;

// following these websites
// codemonkey lobby code <https://youtu.be/-KDlEBfCBiU?si=G2x9tyfwr1eG_ia1&t=2894> 
// codemonkey multiplayer relay tutorial <https://www.youtube.com/watch?v=msPNJ2cxWfw&ab_channel=CodeMonkey>

public class NetworkingLobby : MonoBehaviour
{

    public static NetworkingLobby Instance { get; private set; }

    #region UI Reference Objects

    public Transform networkSpawnClientsContainer;
    public Transform networkLobbyContainer;

    #endregion UI Reference Objects

    #region Unity Lobby Services Variables

    private Lobby hostLob; // holds data for the lobby we create such as players and lobby data
    private Lobby joinedLob; // same as above but if we joined. We always have a joined lobby but we might not always have a host lobby
    private float lobbyHeartbeatTimer = 25f;
    private float lobbyUpdateTimer = 2f;
    private int lobIdSelected = -1;
    QueryResponse queryLobResp;

    public string playerName = ""; // the actual data value (from our data dictionary) we want to read for other purposes
    public string playerIcon = "";
    public int playerIconId = 0;
    public bool isReady = false;


    // usernames randomly generated
    private List<string> possibleNamesPt1 = new List<string>() { "Wild", "Extreme", "GodLike", "Absolute", "Ungodly", "Cheezer", "Deadly", "Uncatchable", "Monster", "Undoubtably", "Remorseful", "Rich", };
    private List<string> possibleNamesPt2 = new List<string>() { "Cat", "Dawg", "Ninja", "Wizard", "Genious", "Cheater", "McSmokes", "Destroyer", "Feeder", "Theologist", "Human", "StuckUp", };
    private List<string> possibleNamesPt3 = new List<string>() { "Jr.", "Sr.", "The First", "The Second", "The Third", "In Training", "The Student", "The Master", "The One and Only", "In Accounting", "JK", "UrMom", };

    // special variables for lobby and player data
    // player
    [HideInInspector] public string data_PlayerName = "PlayerName"; // the name of the data dictionary type
    [HideInInspector] public string data_PlayerIcon = "PlayerIcon";
    [HideInInspector] public string data_PlayerIsReady = "PlayerIsReady";
    // lobby
    [HideInInspector] public string data_LobbyGameMode = "GameMode";
    [HideInInspector] public string data_MapEnvironment = "Map";

    // more reference data for available options
    private List<string> data_Icons = new List<string>() { "icon_char_offline", "icon_char_wizard", "icon_char_warrior", "icon_char_gunner", };
    private List<string> data_GameModes = new List<string>() { "Deathmatch", };
    private List<string> data_MapEnvironments = new List<string>() { "RottenGrove", };

    #endregion unity lobby services variables

    public PlayerCardData ref_PlayerCardData; // helps us update player data when we connect
         

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        //MakeOnlineAccount();
        InitializeVars();
    }

    public void LateUpdate()
    {
        KeepLobbyActive();
        UpdateLobbyServerData();
    }

    private void InitializeVars()
    {
        networkSpawnClientsContainer.gameObject.SetActive(false);
        networkLobbyContainer.gameObject.SetActive(false);
        playerIconId = 0;
    }
    

    #region LOBBY HOST DATA

    public async void CreateLobby(bool _isPrivate) // creates a new lobby in unity systems and allows us to set it private or public
    {
        try
        {
            string lobName = $"{playerName}'s Lobby";
            int lobMaxPlayers = 8;
            CreateLobbyOptions createLobOptions = new CreateLobbyOptions
            {
                IsPrivate = _isPrivate,
                Player = ReturnNewPlayerObj(),
                Data = new Dictionary<string, DataObject> {
                    { data_LobbyGameMode, new DataObject(DataObject.VisibilityOptions.Public, data_GameModes[0], DataObject.IndexOptions.S1) },  // gamemode[0] = Deathmatch
                     { data_MapEnvironment, new DataObject(DataObject.VisibilityOptions.Public, data_MapEnvironments[0], DataObject.IndexOptions.S2) }, // map[0] = RottenGrove
                },
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobName, lobMaxPlayers, createLobOptions);
            hostLob = lobby;
            joinedLob = hostLob;

            Debug.Log($"Created Lobby - {lobby.Name} - {lobby.MaxPlayers} possible player slots \nLobby Id - {lobby.Id} \nJoinCode - {lobby.LobbyCode}");
            PrintPlayersInLobby(joinedLob);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void KeepLobbyActive() // regularly pings the unity systems to keep our created lobby active in search results
    {
        try
        {
            if (hostLob != null)
            {
                lobbyHeartbeatTimer -= Time.deltaTime;

                if (lobbyHeartbeatTimer <= 0)
                {
                    lobbyHeartbeatTimer = 25f;
                    await LobbyService.Instance.SendHeartbeatPingAsync(hostLob.Id);
                    Debug.Log($"Lobby Heartbeat: {hostLob.Name}");
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void UpdateLobbyServerData() // handles updating lobby information to all lobby viewers (can do max 1 request per second)
    {
        if (joinedLob == null)
            return;

        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer <= 0)
        {
            lobbyUpdateTimer = 2f;
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLob.Id);
            joinedLob = lobby;
        }

    }

    public async void UpdateLobbyData(string _gameMode) // allows changing of our hosted lobby's game data like GameMode (can include more variables passed if we want to update more
    {
        if (hostLob == null)
            return;
        try
        {
            hostLob = await Lobbies.Instance.UpdateLobbyAsync(hostLob.Id, new UpdateLobbyOptions // update our hosted lobby
            {
                Data = new Dictionary<string, DataObject> { { data_LobbyGameMode, new DataObject(DataObject.VisibilityOptions.Public, _gameMode) } } // update data for game mode
            });
            joinedLob = hostLob;
            PrintPlayersInLobby(joinedLob);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void DeleteLobby() // allows user (ideally host) to delete the lobby || if all players leave the lobby will close after 30 seconds
    {
        if (hostLob == null)
            return;
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostLob.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void MigrateLobbyHost(int _playersLobbyId) // allows user (ideally host) to migrate host of the lobby || 0 = first player in lobby list, while 1 = 2nd player in lobby list
    {
        try
        {
            hostLob = await Lobbies.Instance.UpdateLobbyAsync(hostLob.Id, new UpdateLobbyOptions // update our hosted lobby
            {
                HostId = joinedLob.Players[0].Id
            });
            joinedLob = hostLob;
            PrintPlayersInLobby(joinedLob);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }    

    public async void KickPlayerFromLobby(int _playersLobbyId ) // allows user (ideally host) to kick players from the lobby || 0 = first player in lobby list, while 1 = 2nd player in lobby list
    {
        if (joinedLob == null)
            return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLob.Id, joinedLob.Players[_playersLobbyId].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    #endregion lobby management data

    #region LOBBY JOIN DATA

    public async void ListLobbies() // Pulls a list of active and public lobbies within our query
    {
        try
        {
            QueryLobbiesOptions queryLobOptions = new QueryLobbiesOptions
            {
                Count = 20, // show first 20 results
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT), // show lobbies with more than 0 slots
                    new QueryFilter(QueryFilter.FieldOptions.S1, data_GameModes[0], QueryFilter.OpOptions.EQ) // show lobbies with gamemode deathmatch
                },

                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created) } // order of ascension is decending
            };
            queryLobResp = await Lobbies.Instance.QueryLobbiesAsync();

            //Debug.Log($"Lobbies found - {queryLobResp.Results.Count}");
            //foreach (Lobby lob in queryLobResp.Results)
            //{
            //    Debug.Log($"Lobby - {lob.Name} - {lob.MaxPlayers} possible player slots - {lob.Data[data_LobbyGameMode].Value} Gamemode");
            //}

            LobbyHandler.Instance.RefreshLobbyList(queryLobResp.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void JoinLobbyId(int _lobId) // joins lobby by an ID, which is most easily gathered by querying the possible lobbies
    {
        lobIdSelected = _lobId;
        try
        {
            if (_lobId == -1 || queryLobResp == null)
                return;

            JoinLobbyByIdOptions joinLobIdOptions = new JoinLobbyByIdOptions { Player = ReturnNewPlayerObj() };
            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryLobResp.Results[_lobId].Id, joinLobIdOptions);
            joinedLob = joinedLobby;
            PrintPlayersInLobby(joinedLob);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void JoinLobbyCode(string _lobJoinCode) // joins a lobby by a short code we are provided
    {
        if (string.IsNullOrEmpty(_lobJoinCode))
            return;
        try
        {
            JoinLobbyByCodeOptions joinLobCodeOptions = new JoinLobbyByCodeOptions { Player = ReturnNewPlayerObj() };
            Lobby joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(_lobJoinCode, joinLobCodeOptions);
            joinedLob = joinedLobby;
            PrintPlayersInLobby(joinedLob);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void QuickJoinLobby() // finds the first available lobby we are able to join
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public async void LeaveLobby() // allows user to leave a lobby they have entered or created || if host leaves Unity has automatic host migration but it's random
    {
        if (joinedLob == null)
            return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLob.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }
    #endregion lobby join data

    #region PLAYER DATA

    public async void MakeOnlineAccount() // sets up our account as a user that can join unity services
    {
        try
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In {AuthenticationService.Instance.PlayerId}");
            };

            // if we do anonomous
            await AuthenticationService.Instance.SignInAnonymouslyAsync();// can instead use this with steam to link accounts
            int RandX = Random.Range(0, possibleNamesPt1.Count - 1);
            int RandY = Random.Range(0, possibleNamesPt2.Count - 1);
            int RandZ = Random.Range(0, possibleNamesPt3.Count - 1);
            playerName = $"{possibleNamesPt1[RandX]} {possibleNamesPt2[RandY]} {possibleNamesPt3[RandZ]}";
            playerIconId++;
            playerIcon = data_Icons[playerIconId];

            // turn on the ui obj so we can find lobbies or make our own match
            networkLobbyContainer.gameObject.SetActive(true);
            if (ref_PlayerCardData)
                ref_PlayerCardData.UpdatePlayerData(); // if we dont have it, it won't update until we try to change our player data / icon through other means
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public void LogOutOnlineAccount() // sets up our account as a user that can join unity services
    {
        try
        {
            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log($"Signed Out {AuthenticationService.Instance.PlayerId}");
            };
            playerIconId = 0;
            playerIcon = data_Icons[playerIconId];

            // turn off the ui obj so we can't find lobbies or make our own match
            networkLobbyContainer.gameObject.SetActive(false);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }

    public Player ReturnNewPlayerObj() // a function to setup player data in unity services constistently. Any data we want in a player is created here
    {
        return new Player
        { // here are the player parameters we're setting / creating
            Data = new Dictionary<string, PlayerDataObject> {
                { data_PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
                { data_PlayerIcon, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerIcon) },
            },
        };

    }

    public async void UpdatePlayerData(string _newName, string _newIcon) // updating player data we send and then save it in services
    {
        if (joinedLob == null)
            return;

        if (!string.IsNullOrEmpty(_newName))
            playerName = _newName;

        if (!string.IsNullOrEmpty(_newIcon))
            playerIcon = _newIcon;

        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(joinedLob.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject> {
                { data_PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
                { data_PlayerIcon, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerIcon) },
            },
            });

            //PlayerCardData.Instance.UpdatePlayerData(); // calls our player card instance and updates data that was changes
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }
    

    public void PrintPlayersInLobby(Lobby _lob) // a log for reading information from the players in a given lobby
    {
        if (_lob == null)
            return;

        Debug.Log($"Lobby Name: {_lob.Name} \nLobby Mode: {_lob.Data[data_LobbyGameMode].Value} \nLobby Map: {_lob.Data[data_MapEnvironment].Value}");
        foreach (Player player in _lob.Players)
        {
            Debug.Log($"Player Data: ...\nID - {player.Id} \nPlayerName - {player.Data[data_PlayerName].Value}");
        }
    }

    public void CheckDataOfJoinedLobby()// testing purposes only for testing updated information after joining a lobby
    {
        if(joinedLob !=null)
            PrintPlayersInLobby(joinedLob);
    }


    public void CycleIconsFromButton()// meant to allow a single click to cycle us through icons for now. || eventually we should just use the function this calls
    {
        playerIconId++;

        if (playerIconId == 0 || playerIconId >= data_Icons.Count)
            playerIconId = 1;

        playerIcon = data_Icons[playerIconId];
        UpdatePlayerData(null, playerIcon);
    }

    #endregion player data

   
} // end of NetworkingLobby Class