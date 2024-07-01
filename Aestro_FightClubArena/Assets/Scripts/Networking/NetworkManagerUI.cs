using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//following https://www.youtube.com/watch?v=3yuBOB3VrCk&ab_channel=CodeMonkey || https://youtu.be/3yuBOB3VrCk?si=DVukWAYhQro0raef&t=1226

/// <summary>
/// <para> Handles Joining / Hosting of servers </para>
/// </summary>
public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button buttonHost, buttonServer, buttonClient;


    private void Awake()
    {
        if (buttonHost)
        {
            buttonHost.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
        }

        if (buttonServer)
        {
            buttonServer.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
        }

        if (buttonClient)
        {
            buttonClient.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
        }
    }// end of Awake()

}// end of NetworkManagerUI class
