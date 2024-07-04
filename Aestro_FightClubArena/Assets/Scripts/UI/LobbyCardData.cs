using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class LobbyCardData : MonoBehaviour // this goes on and handles the lobby card that is populated when we search for lobbies
{
    public Lobby myLobbyReference;
    public string myLobbyName;
    public List<Sprite> possibleGameModeSprites;

    public Image lobbyIcon;
    public TextMeshProUGUI lobbyTitle;
    public TextMeshProUGUI lobbyPlayerCount;
    

    public void UpdateLobbyData(Lobby _lobbyRef)
    {
        myLobbyReference = _lobbyRef;

        if (myLobbyReference == null)
            return;

        //lobbyTitle.text = myLobbyReference.Data[NetworkingLobby.Instance.data_LobbyGameMode].Value;
        lobbyTitle.text = _lobbyRef.Name;
        lobbyPlayerCount.text = $"{myLobbyReference.Players.Count}/{myLobbyReference.MaxPlayers}";

        foreach (Sprite iconImage in possibleGameModeSprites)
        {
            if (iconImage.name == NetworkingLobby.Instance.playerIcon)
            {
                lobbyIcon.sprite = iconImage;
            }
        }

    } 

    public void TryToJoinLobby()
    {
        if(myLobbyReference == null)
            return;

        NetworkingLobby.Instance.JoinLobbyCode(myLobbyReference.LobbyCode);        
    }
}
