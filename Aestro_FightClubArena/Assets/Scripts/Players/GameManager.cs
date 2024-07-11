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
    private Transform player1;
    private Transform player2;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // SetMatchStartData() takes a list of transforms that are either players or the map
    // if bool isMap is true, then the list is just one item, and it's the map
    public void SetMatchStartData(List<Transform> transforms, bool isMap)
    {
        if (isMap && transforms.Count == 1) environment = transforms[0];
        else
        {
            player1 = transforms[0];
            player2 = transforms[1];
        }
    }
}
