using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerCardData : MonoBehaviour // the ui handler for player data when making an account
{
    public static PlayerCardData Instance { get; private set; }

    public List<Sprite> possibleGameModeSprites;
    public string playerId;
    public Image playerIconImg;
    public TextMeshProUGUI playerTitle;

    private void Awake()
    {
        Instance = this;        
    }

    public void UpdatePlayerData()
    {

        if (string.IsNullOrEmpty(NetworkingLobby.Instance.playerName))
            return;

        //lobbyTitle.text = myLobbyReference.Data[NetworkingLobby.Instance.data_LobbyGameMode].Value;
        playerTitle.text = NetworkingLobby.Instance.playerName;

        foreach (Sprite iconImage in possibleGameModeSprites)
        {
            if (iconImage.name == NetworkingLobby.Instance.playerIcon)
            {
                playerIconImg.sprite = iconImage;
            }
        }
        playerId = NetworkingLobby.Instance.playerId;
    }

}
