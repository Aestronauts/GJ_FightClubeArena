using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;


public class LobbyHandler : MonoBehaviour // the UI handler for lobby population and updating
{
    public static LobbyHandler Instance { get; private set; }

    [Tooltip("The Root Canvas Object that holds the lobby Obj")]
    [SerializeField] private Transform lobbyRootContainer;
    [Tooltip("The Root Canvas Object that is the lobby Obj and will be duplicated")]
    [SerializeField] private Transform lobbyCardTemplate;

    [Tooltip("The Root Canvas Object that holds the room Obj (different than the lobbby which is all lobbies, the room is a single lobby's details)")]
    [SerializeField] private Transform roomRootContainer;
    [Tooltip("The Root Canvas Object that is the room Obj and will be duplicated")]
    [SerializeField] private Transform roomCardTemplate;
    [Tooltip("The Root Canvas Object that holds the Lobby buttons to change settings or start-up a game)")]
    [SerializeField] private Transform roomOptionsContianer;

    private void Awake()
    {
        if (LobbyHandler.Instance != null && LobbyHandler.Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        lobbyCardTemplate.gameObject.SetActive(false);
        roomCardTemplate.gameObject.SetActive(false);
    }

    public void RefreshLobbyList(List<Lobby> _lobbyList)
    {
        if (lobbyRootContainer)
            lobbyRootContainer.gameObject.SetActive(true);

        foreach (Transform child in lobbyRootContainer) // remove current entries
        {
            if (child == lobbyCardTemplate)
                continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobbyFound in _lobbyList)
        {
            Transform lobbySingleTransform = Instantiate(lobbyCardTemplate, lobbyRootContainer);
            lobbySingleTransform.gameObject.SetActive(true);
            LobbyCardData ref_lobbyCardData = null;
            lobbySingleTransform.TryGetComponent<LobbyCardData>(out ref_lobbyCardData);
            // send the lobby information for this specific lobby
            if (ref_lobbyCardData)
                ref_lobbyCardData.UpdateLobbyData(lobbyFound);
        }

    }

    public void RefreshRoomData(Lobby _joinedLobby)
    {
        if (roomRootContainer)
            roomRootContainer.gameObject.SetActive(true);
        else
            return;

        foreach (Transform child in roomRootContainer) // remove current entries
        {
            if (child == roomCardTemplate)
                continue;
            Destroy(child.gameObject);
        }

        foreach (Player playerFound in _joinedLobby.Players)
        {
            Transform roomSingleTransform = Instantiate(roomCardTemplate, roomRootContainer);
            roomSingleTransform.gameObject.SetActive(true);
            RoomCardData ref_roomCardData = null;
            roomSingleTransform.TryGetComponent<RoomCardData>(out ref_roomCardData);
            // send the player information for this specific room card
            if (ref_roomCardData)
                ref_roomCardData.UpdatePlayerData(playerFound);
            if (playerFound.Id == PlayerCardData.Instance.playerId)
                ref_roomCardData.playerReadyToggle.interactable = true;
        }

        if (NetworkingLobby.Instance.hostLob != null && roomOptionsContianer)
            roomOptionsContianer.gameObject.SetActive(true);
        else if (NetworkingLobby.Instance.hostLob == null && roomOptionsContianer)
            roomOptionsContianer.gameObject.SetActive(false);

    }

    public void DeleteReferenceObjs()
    {
        //Destroy(lobbyRootContainer);
    }

}
