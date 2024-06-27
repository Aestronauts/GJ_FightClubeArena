using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCharacterManagerFAKE : NetworkBehaviour
{
    //script refs
    public PlayerInputHandler ref_PlayerInputHandler;
    public NetworkObject ref_NetworkObject;
    public NetworkManager ref_NetworkManager;

    // network variables
    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // Owner VS Server (owner is write your own, server is change others)


    private void Awake()
    {
        if (GameObject.FindAnyObjectByType<NetworkManager>() != null)
            ref_NetworkManager = GameObject.FindAnyObjectByType<NetworkManager>();

        //if we dont have ref to network manager we are offline
        if (!ref_NetworkManager)
        {
            ref_PlayerInputHandler.enabled = true;
            ref_NetworkObject.enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //if (!IsOwner) // requires NetworkBehavior to check if this is you (the player)
        //    return;
        if (!ref_NetworkManager)
            return;


        if (ref_PlayerInputHandler)
            ref_PlayerInputHandler.enabled = IsOwnedByServer;
        if (ref_NetworkObject)
            ref_NetworkObject.enabled = IsOwnedByServer;
    }

}// PlayerCharacterManagerFAKE class
