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
        

        if (_lobbyRef == null)
            return;

        myLobbyReference = _lobbyRef;

        //lobbyTitle.text = myLobbyReference.Data[NetworkingLobby.Instance.data_LobbyGameMode].Value;
        myLobbyName = _lobbyRef.Name;
        lobbyTitle.text = _lobbyRef.Name;
        lobbyPlayerCount.text = $"{myLobbyReference.Players.Count}/{myLobbyReference.MaxPlayers}";

        foreach (Sprite iconImage in possibleGameModeSprites)
        {
            if (iconImage.name == _lobbyRef.Data[NetworkingLobby.Instance.data_LobbyIcon].Value)
            {
                lobbyIcon.sprite = iconImage;
            }
        }

    } 

    public void TryToJoinLobby()
    {
        if(myLobbyReference == null)
            return;

        NetworkingLobby.Instance.JoinLobbyId(myLobbyReference.Id);
        print($"tried to join by lobby ID: {myLobbyReference.Id}");
    }
}
