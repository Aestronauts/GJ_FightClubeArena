using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public  static GameManager Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = new GameObject().AddComponent<GameManager>();
                _Instance.name = _Instance.GetType().ToString();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    //public GameObject Environment1;
    //public GameObject Environment2;
    //public GameObject FireMage;
    //public GameObject Character2;
    //public GameObject Character3;

    private Transform environment;
    public Transform player1, player1_Obj;
    public Transform player2, player2_Obj;

    private List<Transform> spawnedEnvironments = new List<Transform>();

    private Vector3 environmentSpawnLocation = new Vector3(0,0,0);
    private Vector3 player1SpawnLocation = new Vector3(7, -1, 3);
    private Vector3 player2SpawnLocation = new Vector3(-3, -1, -3);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // SetMatchStartData() takes a list of transforms that are both players' models and
    //      a singular tranform that is the environment map to spawn
    // if bool isMap is true, then the list is just one item, and it's the map
    public void SetMatchStartData(List<Transform> playerTransforms, Transform mapTransform)
    {
        player1_Obj = playerTransforms[0];
        player2_Obj = playerTransforms[1];
        environment = mapTransform;

        SpawnPlayers();
        SpawnEnvironment();

        //if (isMap && transforms.Count == 1)
        //{
        //    environment = transforms[0];
        //    SpawnEnvironment();
        //}
        //else
        //{
        //    player1 = transforms[0];
        //    player2 = transforms[1];
        //    SpawnPlayers();
        //}
    }

    private void SpawnEnvironment()
    {
        Transform newEnvironment = Instantiate(environment, environmentSpawnLocation, Quaternion.identity);
        newEnvironment.transform.Rotate(0, -90, 0);
        spawnedEnvironments.Add(newEnvironment);
    }

    private void SpawnPlayers()
    {
        player1 = Instantiate(player1_Obj, player1SpawnLocation, Quaternion.identity);
        player2 = Instantiate(player2_Obj, player2SpawnLocation, Quaternion.identity);
    }
}
