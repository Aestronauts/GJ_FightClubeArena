using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerJoiner : NetworkBehaviour
{
    [Header("ONLY IF WE DIDNT HAVE ENOUGH TIME")]
    [Tooltip("Nearing the end of the project Noah and Millie were working together to fix some networking ability issues likely related to ownership. To do this, Noah decided it would be best to refactor 4 scripts down to 3, and it takes more time than we had if you're still seeing this.")]
    public bool useLegacyAbilitySpawning = true;

    [Header("REALTIME SCRIPT REFERENCES\n____________________")]
    // script refs to our own variables
    public NetworkObject ref_NetworkObject;    
    public NetworkManager ref_NetworkManager;
    public PlayerInputHandler ref_PlayerInputHandler;


    [Space]
    [Header("NETWORK VARIABLES\n____________________")]
    // Online Connection
    private bool isOnline;
    // we should have spawned in
    private bool sentSpawnCharacterSignal, sentLocalAssetSpawnSignal;
    // should we use pooling
    private bool useAbilityPooling = true;
    // HEALTH
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); // Owner VS Server (owner is write your own, server is change others)
    


    [Space]
    [Header("REALTIME ASSET REFERENCES\n____________________")]
    // ref to all clients that have joined
    //public List<NetworkPlayerJoiner> playersJoined_NetObjs = new List<NetworkPlayerJoiner>();   
    // selected Character model
    public int characterModelSelected;
    // reference to playerModelSpawned
    public Transform spawnedCharacterModel;
    // the selected map model we'll be using
    public int mapModelSelected;
    // reference to map model we spawned
    public Transform spawnedMapModel;
    // the drawing assets we require for casting spells
    public Transform spawnedDrawingObjPrefab, spawnedDrawingCamPrefab;
    // the target dummy we spawn
    public Transform spawnedTargetDummy;
    // the list of models we spawn for abilities
    public List<Transform> spawnedAbilitiesPool = new List<Transform>();


    [Space]
    [Header("STORED ASSET REFERENCES\n____________________")]
    // drawing stuff
    [SerializeField]
    private Transform transDrawingObjPrefab;
    [SerializeField]
    private Transform transDrawingCamPrefab;
    [SerializeField]
    private Transform transAbilityPrefab;
    // an example of a character we might spawn
    [SerializeField]
    private Transform[] transCharacterModels;
    // an example of the maps we might want to spawn (can do so locally)
    [SerializeField]
    private Transform[] transMapModels;
    // target dummy for testing abilities and damage
    [SerializeField]
    private Transform transTargetDummyModel;

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

        }

        if (ref_NetworkObject)
            Debug.Log($"OWNER CHECK FOR: {transform.name} \nIsOwner: {ref_NetworkObject.IsOwner} \nIsOwnedByServer: {ref_NetworkObject.IsOwnedByServer} \nisHost: {IsHost} \nOwnerClientID: {ref_NetworkObject.OwnerClientId} \nServerIsHost: {ServerIsHost}");

    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (IsHost)
            Debug.Log("I AM SERVER HOST");

        if (!sentLocalAssetSpawnSignal)
        {
            SpawnLocalAssets();
            sentLocalAssetSpawnSignal = true;
        }

        //if (ServerIsHost && !spawnedTargetDummy && transTargetDummyModel)
        //{
        //    spawnedTargetDummy = Instantiate(transTargetDummyModel);
        //    spawnedTargetDummy.GetComponent<NetworkObject>().Spawn(true);
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sentSpawnCharacterSignal = true;
            Debug.Log("Pressing Space");
            if (isOnline && !spawnedCharacterModel)
            {
                Debug.Log("Spawning Character");
                SpawnCharacterServerRpc(new ServerRpcParams());

                CheckAllJoiners();
            }
        }

        if (!spawnedCharacterModel && sentSpawnCharacterSignal)
        {
            CheckAllJoiners();
        }
    }

    public void SpawnLocalAssets()
    {
        //spawn our only environment locally
        if (!spawnedMapModel && transMapModels[mapModelSelected])
            spawnedMapModel = Instantiate(transMapModels[mapModelSelected]);
        if (spawnedMapModel)
            spawnedMapModel.transform.Rotate(0, -90, 0);
        // spawn our drawing references locally
        if (!spawnedDrawingObjPrefab && transDrawingObjPrefab)
            spawnedDrawingObjPrefab = Instantiate(transDrawingObjPrefab);
        if (!spawnedDrawingCamPrefab && transDrawingCamPrefab)
            spawnedDrawingCamPrefab = Instantiate(transDrawingCamPrefab);
        if (spawnedDrawingObjPrefab && spawnedDrawingCamPrefab)
        {
            spawnedDrawingCamPrefab.position = Camera.main.transform.position;
            spawnedDrawingCamPrefab.rotation = Camera.main.transform.rotation;
            spawnedDrawingObjPrefab.GetComponent<DrawOnScreen>().strokesCamera = spawnedDrawingCamPrefab.GetComponent<Camera>();
        }
    }

    public void CheckAllJoiners()
    {
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
            NetworkObject storeRef = null;

            foreach (PlayerInputHandler _player in allOtherPlayerObj)
            {
                _player.TryGetComponent<NetworkObject>(out storeRef);

                if (storeRef && storeRef.OwnerClientId == ref_NetworkObject.OwnerClientId)
                {
                    spawnedCharacterModel = _player.transform;
                    ref_PlayerInputHandler = _player;
                    ref_PlayerInputHandler.playerJoiner = this;
                }
                else
                    _player.enabled = false;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHealth.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log($"Client: {OwnerClientId} - HP was:{previousValue} - HP is: {newValue}");
            playerHealth.Value = ref_PlayerInputHandler.gameObject.GetComponent<PlayerHealthManager>().currentPlayerHealth;
        };
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerHealth.OnValueChanged += (int previousValue, int newValue) => { };
    }


    [ServerRpc(RequireOwnership = true)]
    private void SpawnCharacterServerRpc(ServerRpcParams _serverRpcParams) // spawn player model
    {
        Debug.Log($"ClientOwner: {OwnerClientId}\nSenderClient - {_serverRpcParams.Receive.SenderClientId}\n Spawning Model - {transCharacterModels[characterModelSelected].name}");

        if (!transCharacterModels[characterModelSelected])
            return;

        spawnedCharacterModel = Instantiate(transCharacterModels[characterModelSelected], transform.position, transCharacterModels[characterModelSelected].rotation);
        spawnedCharacterModel.TryGetComponent<NetworkObject>(out ref_NetworkObject);
        if (ref_NetworkObject)
        {
            ref_NetworkObject.Spawn(true);// can despawn or delete   
            if (ref_NetworkObject.OwnerClientId != _serverRpcParams.Receive.SenderClientId)
                ref_NetworkObject.ChangeOwnership(_serverRpcParams.Receive.SenderClientId);
            
        }
        spawnedCharacterModel.TryGetComponent<PlayerInputHandler>(out ref_PlayerInputHandler);
        ref_PlayerInputHandler.enabled = IsOwner;      
        ref_PlayerInputHandler.gameObject.GetComponent<PlayerHealthManager>().networkedPlayer = this;
        playerHealth.Value = ref_PlayerInputHandler.gameObject.GetComponent<PlayerHealthManager>().currentPlayerHealth;

    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnAbilityServerRpc(int _abilitId, Vector3 drawResultValue, ServerRpcParams _serverRpcParams) // spawn player ability
    {
        // CALL ABILITY MANAGER
        // GRAB AND INSTATIATE REFERENCE
        // SET NETWORKOBJECT .SPAWN (TRUE)
        
        //cast ability origin point
        Transform spawnPoint = ref_PlayerInputHandler.abilitySpawnPoint;
        //Animator animator = player_gameObject.GetComponent<Animator>();
        Animator animator = ref_PlayerInputHandler.modelAnimator;
        if (animator == null) { Debug.LogError("Animator not found on PlayerCharacterManager"); }

        if (useLegacyAbilitySpawning)
        {

            // if we can get all our ability references then spawn the ability
            Transform spawnedAbilityClone = null;
            AbilityBehavior cloneBehavior = null;
            //if we are using pooling
            if (useAbilityPooling && spawnedAbilitiesPool.Count > 0)
            {
                foreach (Transform _poolObj in spawnedAbilitiesPool)
                {
                    if (!_poolObj)
                        continue;
                    _poolObj.TryGetComponent<AbilityBehavior>(out cloneBehavior);
                    if (cloneBehavior && cloneBehavior.abilityID == _abilitId && _poolObj.gameObject.activeSelf == false)
                    {
                        spawnedAbilityClone = _poolObj;
                        spawnedAbilityClone.gameObject.SetActive(true);
                        break;
                    }
                    else
                        cloneBehavior = null;
                }
            }

            if (!spawnedAbilityClone && AbilityManager.instance && AbilityManager.instance.SpawnAbilityById(_abilitId))
            {
                // spawn our ability
                spawnedAbilityClone = Instantiate(AbilityManager.instance.SpawnAbilityById(_abilitId)).transform;
                spawnedAbilitiesPool.Add(spawnedAbilityClone);
            }

            // assign the ability behavior         
            if (cloneBehavior)
            {
                cloneBehavior.distanceTraveled = 0f;
                cloneBehavior.spawnLocation = spawnPoint.position;
                cloneBehavior.endLocation = drawResultValue;
            }

            // reference the online capabilities
            NetworkObject cloneNetworkObj = null;
            if (spawnedAbilityClone)
                spawnedAbilityClone.TryGetComponent<NetworkObject>(out cloneNetworkObj);
            if (cloneNetworkObj)
                cloneNetworkObj.Spawn(true);

            if (animator)
            {
                Debug.Log("ASK BRETT about changing these to be standardized / scalable");
                //string newAbilityUse = "Attack_" + _abilitId.ToString(); // --> we could instead do this if the ability animator strings are all "Attack_#"
                switch (_abilitId)
                {
                    case 0:
                        animator.SetTrigger("isBasicAttacks");
                        break;
                    case 1:
                        animator.SetTrigger("isFirePillar");
                        break;
                    case 2:
                        animator.SetTrigger("isTwinFlames");
                        break;
                    default:
                        Debug.Log($"We Tried To Spawn Ability: {_abilitId} - \nWhich Doesn't have an animation condition \nScript: NetworkPlayerJoiner - Function: SpawnAbilityServerRpc");
                        return;
                }
            }
        } //-----------------------------Might Be Able To Simplify Below Here----------------------------------------------------------------------------||
        else
        {


            // Choose "if" logic by matching the given ability name
            // TODO: Is there a better way of doing this???
            if (_abilitId == 0)
            {
                AbilitiesHelper.SpawnAbility(ref_PlayerInputHandler.gameObject, spawnPoint.position, drawResultValue,
                    AbilityManager.instance.FireboltProjectileList,
                    AbilityManager.instance.ProjectilesHolder, this, AbilityManager.instance.abilitiesList, _abilitId);
                if (animator)
                    animator.SetTrigger("isBasicAttacks");

            }
            else if (_abilitId == 1)
            {
                AbilitiesHelper.SpawnAbility(ref_PlayerInputHandler.gameObject, drawResultValue, drawResultValue,
                    AbilityManager.instance.FirePillarProjectileList,
                    AbilityManager.instance.ProjectilesHolder, this, AbilityManager.instance.abilitiesList, _abilitId);
                if (animator)
                    animator.SetTrigger("isFirePillar");
                PlayerCameraManager.instance.ShakeCamera(1f, .5f, .6f);
            }
            else if (_abilitId == 2)
            {
                AbilitiesHelper.SpawnAbility(ref_PlayerInputHandler.gameObject, spawnPoint.position, drawResultValue,
                    AbilityManager.instance.TwinFireboltProjectileList,
                    AbilityManager.instance.ProjectilesHolder, this, AbilityManager.instance.abilitiesList, _abilitId);
                if (animator)
                    animator.SetTrigger("isTwinFlames");
            }


            //PlayerCharacterManager.instance.CastAbility(ref_PlayerInputHandler.gameObject, drawResultValue, _abilitId);
            //ref_NetworkObject.Spawn(true);

            //transAbilityClone.GetComponent<NetworkObject>().Spawn(true); // can despawn or delete
            //Transform transAbilityClone = Instantiate(transAbilityPrefab, spawnedCharacterModel.position, transAbilityPrefab.rotation);
            //AbilityBehavior ref_Temp_Projectile = transAbilityClone.GetComponent<AbilityBehavior>();
            //ref_Temp_Projectile.spawnLocation = spawnedCharacterModel.position;
            //ref_Temp_Projectile.endLocation = new Vector3(0, 0, 0);
        }
    }


}
