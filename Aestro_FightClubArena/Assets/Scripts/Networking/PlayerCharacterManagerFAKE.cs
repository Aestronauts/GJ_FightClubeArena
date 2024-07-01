using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

// animation synching tutorial <https://youtu.be/3yuBOB3VrCk?si=KnmvpwZW_9Di9Y6O&t=2915>
// unity netcode documentation <https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/>
// NAT PunchThrough <https://docs-multiplayer.unity3d.com/netcode/current/learn/listen-server-host-architecture/#what-is-a-listen-server>

public class PlayerCharacterManagerFAKE : NetworkBehaviour
{
    [Header("REALTIME SCRIPT REFERENCES\n____________________")]
    // script refs to our own variables
    public PlayerInputHandler ref_PlayerInputHandler;
    public NetworkObject ref_NetworkObject;
    public NetworkManager ref_NetworkManager;

    [Space]
    [Header("NETWORK VARIABLES\n____________________")]
    // HEALTH
    private NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // Owner VS Server (owner is write your own, server is change others)



    [Space]
    [Header("REALTIME ASSET REFERENCES\n____________________")]
    // ref to all clients that have joined
    public List<NetworkObject> playersJoined_NetObjs = new List<NetworkObject>();
    // ref to all UI Lobby Cards we've spawned
    public List<Transform> playerLobbyCardsList = new List<Transform>();
    // reference to playerModelSpawned
    public Transform spawnedCharacterModel;


    [Space]
    [Header("STORED ASSET REFERENCES\n____________________")]
    // the cnavas object that holds all the players who joined
    [SerializeField]
    private Transform playerLobbyList;
    // the prefab for when someone joins
    [SerializeField]
    private Transform playerLobbyCard;
    // an example of an ability we might spawn
    [SerializeField]
    private Transform transAbilityPrefab;
    // an example of a character we might spawn
    [SerializeField]
    private Transform transCharWizard;

    private void Awake()
    {
        if (!ref_NetworkManager && GameObject.FindAnyObjectByType<NetworkManager>() != null)
            ref_NetworkManager = GameObject.FindAnyObjectByType<NetworkManager>();

        //if we dont have ref to network manager we are offline
        if (!ref_NetworkManager)
        {
            if (ref_PlayerInputHandler)
                ref_PlayerInputHandler.enabled = true;
            if (ref_NetworkObject)
                ref_NetworkObject.enabled = false;
        }

        foreach (Transform child in playerLobbyList)
            child.gameObject.SetActive(false);

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
            SpawnCharacterServerRpc(new ServerRpcParams());
        if (Input.GetKeyDown(KeyCode.Alpha0))
            PlayerJoinedServerRpc(new ServerRpcParams());
        //GeneralClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } }); // sends a message to the client (1)
    }


    public void NewPlayer(NetworkObject _netObj)
    {
        playersJoined_NetObjs.Add(_netObj);
    }

    //[ServerRpc]
    //private void GeneralServerRpc(ServerRpcParams _serverRpcParams) // this is where we put commands that the server / host will be asked to the source of truth (like abilities)
    //{
    //    if (_serverRpcParams.Receive.SenderClientId == OwnerClientId) // Message was sent by us
    //        Debug.Log($"GeneralServerRPC: Sender Client = {OwnerClientId} (which was us)");
    //    else
    //        Debug.Log($"GeneralServerRPC: Sender Client = {_serverRpcParams.Receive.SenderClientId} (which was NOT us)");
    //}

    [ServerRpc]
    private void PlayerJoinedServerRpc(ServerRpcParams _serverRpcParams) // spawn associated UI
    {
        bool ableToJoin = false;
        int uiCardID = 0;
        foreach( Transform child in playerLobbyList)
        {
            if(child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true);
                ableToJoin = true;
                break;
            }
            uiCardID++;
        }

        if (!ableToJoin)
            return;

        Transform spawnedUIObj = playerLobbyList.GetChild(uiCardID);
        playerLobbyCardsList.Add(spawnedUIObj);

        if (playersJoined_NetObjs[playersJoined_NetObjs.Count-1].OwnerClientId == OwnerClientId) // if the sender is also the owner of this client
        {
            foreach(Transform child in playerLobbyCard)
            {
                if (child.GetComponent<Toggle>())
                    child.GetComponent<Toggle>().enabled = true;
            }
        }
    }


    [ServerRpc]
    private void SpawnCharacterServerRpc(ServerRpcParams _serverRpcParams) // spawn player model
    {
        spawnedCharacterModel = Instantiate(transCharWizard, transform.position, transCharWizard.rotation);
        spawnedCharacterModel.TryGetComponent<NetworkObject>(out ref_NetworkObject);
        if (ref_NetworkObject)
            ref_NetworkObject.Spawn(true);// can despawn or delete   
        spawnedCharacterModel.TryGetComponent<PlayerInputHandler>(out ref_PlayerInputHandler);
        Awake();
    }

    [ServerRpc]
    private void SpawnAbilityServerRpc(ServerRpcParams _serverRpcParams) // spawn player ability
    {
        Transform transAbilityClone = Instantiate(transAbilityPrefab, spawnedCharacterModel.position, transAbilityPrefab.rotation);
        transAbilityClone.GetComponent<NetworkObject>().Spawn(true); // can despawn or delete
        Temp_Projectile ref_Temp_Projectile = transAbilityClone.GetComponent<Temp_Projectile>();
        ref_Temp_Projectile.spawnLocation = spawnedCharacterModel.position;
        ref_Temp_Projectile.endLocation = new Vector3(0, 0, 0);
    }


    [ClientRpc]
    private void GeneralClientRpc(ClientRpcParams _clientRpcParams) // for commands or inputs that can be sent to any clients specifically
    {
        // if(_clientRpcParams.Receive.TargetClientId != OwnerClientId) // We are not the host
        Debug.Log($"GeneralClientRPC: Client Sender Target(s) = {_clientRpcParams.Send.TargetClientIds}");
    }

}// PlayerCharacterManagerFAKE class
