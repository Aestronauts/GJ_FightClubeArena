using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// <para> the specific data that each card in a networking lobby references </para>
/// </summary>
public class RoomCardData : MonoBehaviour
{

    public List<Sprite> possibleGameModeSprites;
    public Image playerIconImg;
    public Toggle playerReadyToggle;
    public TextMeshProUGUI playerTitle;
    public bool playerIsReady;
    private string playerIsReadyText = "false";

    public void UpdatePlayerData(Player _playerPassed)
    {
        if (_playerPassed == null)
            return;

        playerIsReadyText = _playerPassed.Data[NetworkingLobby.Instance.data_PlayerIsReady].Value.ToLower();

        if (playerIsReadyText == "true")
            playerIsReady = true;
        else
            playerIsReady = false;

        playerReadyToggle.isOn = playerIsReady;
        playerTitle.text = _playerPassed.Data[NetworkingLobby.Instance.data_PlayerName].Value;

        foreach (Sprite iconImage in possibleGameModeSprites)
        {
            if (iconImage.name == _playerPassed.Data[NetworkingLobby.Instance.data_PlayerIcon].Value)
            {
                playerIconImg.sprite = iconImage;
            }
        }

        UpdateReadyCheck();
    }

    public void UpdateReadyCheck() // this needs to tell the networking lobby that we are ready
    {
        playerIsReady = playerReadyToggle.isOn;

        if (playerIsReady)
            playerIsReadyText = "true";
        else
            playerIsReadyText = "false";

        if (NetworkingLobby.Instance.joinedLob != null)
        {
            //LobbyHandler.Instance.RefreshRoomData(NetworkingLobby.Instance.joinedLob);
            NetworkingLobby.Instance.UpdatePlayerData(null, null, playerIsReadyText);
        }
    }


}
