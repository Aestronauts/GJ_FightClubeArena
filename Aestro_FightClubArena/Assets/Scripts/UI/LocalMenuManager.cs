using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMenuManager : MonoBehaviour
{
    public static LocalMenuManager instance;
    public GameObject menu;
    
    [HideInInspector]public NetworkPlayerJoiner playerJoiner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeaveLobby()
    {
        playerJoiner.ref_NetworkObject.Despawn(true);
    }
}
