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
    public NetworkManager ref_NetworkManager;
    public PlayerInputHandler ref_PlayerInputHandler;


    [Space]
    [Header("NETWORK VARIABLES\n____________________")]
    // Online Connection
    private bool isOnline;
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
    [SerializeField]
    private Transform transAbilityPrefab;
    // an example of a character we might spawn
    [SerializeField]
    private Transform transCharWizard;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!ref_NetworkObject && GetComponent<NetworkObject>() != null)
            ref_NetworkObject = transform.GetComponent<NetworkObject>();

        if (!ref_NetworkManager && GameObject.FindAnyObjectByType<NetworkManager>() != null)
            ref_NetworkManager = GameObject.FindAnyObjectByType<NetworkManager>();


        // if we dont have ref to network manager we are offline
        if (!ref_NetworkManager)
        {
            Debug.Log("You Are Offline Right Now");
            isOnline = false;

            if (ref_NetworkObject)
                ref_NetworkObject.enabled = false;
        }
        // if we do have our network manager
        else
        {
            Debug.Log("You Are Online Right Now");
            isOnline = true;
            #region checking for others
            var allJoinerObjects = FindObjectsOfType<NetworkPlayerJoiner>();
            var allOtherPlayerObj = FindObjectsOfType<PlayerInputHandler>();

            if (allJoinerObjects.Length > 0)
            {
                foreach (NetworkPlayerJoiner _joiner in allJoinerObjects)
                {
                    if (_joiner != this)
                        _joiner.enabled = false;
                }
            }
            if (allOtherPlayerObj.Length > 0)
            {
                foreach (PlayerInputHandler _player in allOtherPlayerObj)
                {
                    _player.enabled = false;
                }
            }
        }
        #endregion

        //if (ref_PlayerInputHandler)
        //    ref_PlayerInputHandler.enabled = IsOwner;
        //if (ref_NetworkObject)
        //    ref_NetworkObject.enabled = IsOwner;

        SpawnCharacterServerRpc(new ServerRpcParams());
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

    public void NewPlayer(NetworkObject _netObj)
    {
        playersJoined_NetObjs.Add(_netObj);
    }

    [ServerRpc]
    private void SpawnCharacterServerRpc(ServerRpcParams _serverRpcParams) // spawn player model
    {
        spawnedCharacterModel = Instantiate(transCharWizard, transform.position, transCharWizard.rotation);
        spawnedCharacterModel.TryGetComponent<NetworkObject>(out ref_NetworkObject);
        if (ref_NetworkObject)
            ref_NetworkObject.Spawn(true);// can despawn or delete   
        spawnedCharacterModel.TryGetComponent<PlayerInputHandler>(out ref_PlayerInputHandler);
        //Awake();
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


}
