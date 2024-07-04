using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class RoomCardData : MonoBehaviour
{        
    public List<Sprite> possibleGameModeSprites;

    public Image playerRoomIcon;
    public TextMeshProUGUI lobbyTitle;
    public bool isReady;

    public void UpdatePlayerData(Player _playerPassed)
    {

    }
}
