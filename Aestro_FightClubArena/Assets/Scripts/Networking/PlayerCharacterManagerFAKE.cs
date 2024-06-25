using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCharacterManagerFAKE : NetworkBehaviour
{
    //script refs
    public PlayerInputHandler ref_PlayerInputHandler;


    // network variables
    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // Owner VS Server (owner is write your own, server is change others)

  
    // Update is called once per frame
    private void Update()
    {
        //if (!IsOwner) // requires NetworkBehavior to check if this is you (the player)
        //    return;

        if (ref_PlayerInputHandler)
            ref_PlayerInputHandler.enabled = IsOwnedByServer;       
    }

}// PlayerCharacterManagerFAKE class
