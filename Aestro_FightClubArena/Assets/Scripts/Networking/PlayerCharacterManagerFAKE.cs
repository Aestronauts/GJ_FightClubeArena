using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// animation synching tutorial <https://youtu.be/3yuBOB3VrCk?si=KnmvpwZW_9Di9Y6O&t=2915>
// unity netcode documentation <https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/>
// NAT PunchThrough <https://docs-multiplayer.unity3d.com/netcode/current/learn/listen-server-host-architecture/#what-is-a-listen-server>

public class PlayerCharacterManagerFAKE : NetworkBehaviour
{
    //script refs
    public PlayerInputHandler ref_PlayerInputHandler;
    public NetworkObject ref_NetworkObject;
    public NetworkManager ref_NetworkManager;

    // network variables
    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // Owner VS Server (owner is write your own, server is change others)

    [SerializeField]
    private Transform transAbilityPrefab;

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHealth.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log($"Client: {OwnerClientId} - HP was:{previousValue} - HP is: {newValue}");
        };
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerHealth.OnValueChanged += (int previousValue, int newValue) => { };
    }

    // Update is called once per frame
    private void Update()
    {
        if (!ref_NetworkManager) // normally - "if (!IsOwner)" - but we currently check using an alternative method
            return;

        if (ref_PlayerInputHandler)
            ref_PlayerInputHandler.enabled = IsOwner;
        if (ref_NetworkObject)
            ref_NetworkObject.enabled = IsOwner;

        if (Input.GetKeyDown(KeyCode.Alpha8))
            SpawnAbilityServerRpc(new ServerRpcParams());
        if (Input.GetKeyDown(KeyCode.Alpha9))
            GeneralServerRpc(new ServerRpcParams());
        if (Input.GetKeyDown(KeyCode.Alpha0))
            GeneralClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } }); // sends a message to the client (1)
    }

    [ServerRpc]
    private void SpawnAbilityServerRpc(ServerRpcParams _serverRpcParams) // can pass in a parameter for deciding what to spawn
    {
        Transform transAbilityClone = Instantiate(transAbilityPrefab, transform.position, transAbilityPrefab.rotation);
        transAbilityClone.GetComponent<NetworkObject>().Spawn(true); // can despawn or delete
        Temp_Projectile ref_Temp_Projectile = transAbilityClone.GetComponent<Temp_Projectile>();
        ref_Temp_Projectile.spawnLocation = transform.position;
        ref_Temp_Projectile.endLocation = new Vector3(0, 0, 0);
    }

    [ServerRpc]
    private void GeneralServerRpc(ServerRpcParams _serverRpcParams) // this is where we put commands that the server / host will be asked to the source of truth (like abilities)
    {
        if (_serverRpcParams.Receive.SenderClientId == OwnerClientId) // Message was sent by us
            Debug.Log($"GeneralServerRPC: Sender Client = {OwnerClientId} (which was us)");
        else
            Debug.Log($"GeneralServerRPC: Sender Client = {_serverRpcParams.Receive.SenderClientId} (which was NOT us)");
    }

    [ClientRpc]
    private void GeneralClientRpc(ClientRpcParams _clientRpcParams) // for commands or inputs that can be sent to any clients specifically
    {
        // if(_clientRpcParams.Receive.TargetClientId != OwnerClientId) // We are not the host
        Debug.Log($"GeneralClientRPC: Client Sender Target(s) = {_clientRpcParams.Send.TargetClientIds}");
    }

}// PlayerCharacterManagerFAKE class
