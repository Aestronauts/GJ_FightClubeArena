using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerJoiner : NetworkBehaviour
{
    [Header("REALTIME SCRIPT REFERENCES\n____________________")]
    // script refs to our own variables
    public NetworkObject ref_NetworkObject;
    public PlayerCharacterManagerFAKE ref_PlayerCharacterManagerFAKE;
    public NetworkManager ref_NetworkManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!ref_NetworkObject)
            ref_NetworkObject = transform.GetComponent<NetworkObject>();

        if (!ref_NetworkManager && GameObject.FindAnyObjectByType<NetworkManager>() != null)
            ref_NetworkManager = GameObject.FindAnyObjectByType<NetworkManager>();

        if (!ref_PlayerCharacterManagerFAKE && GameObject.FindAnyObjectByType<PlayerCharacterManagerFAKE>() != null)
            ref_PlayerCharacterManagerFAKE = GameObject.FindAnyObjectByType<PlayerCharacterManagerFAKE>();

        // if we dont have ref to network manager we are offline
        if (!ref_NetworkManager)
        {
            if (ref_NetworkObject)
                ref_NetworkObject.enabled = false;
        }
        // if we do have our network manager
        else
        {
            // if we do have our player character manager then add ourselves to the list
            if (ref_PlayerCharacterManagerFAKE)
                ref_PlayerCharacterManagerFAKE.NewPlayer(ref_NetworkObject);

        }


    }

}
