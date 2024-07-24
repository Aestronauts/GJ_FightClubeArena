using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// <para> The handler for connecting to Unity's Relay servers (has pricing plans based on CCU per month and other variables</para>
/// </summary>
// <https://unity.com/products/relay>

public class NetworkingRelayManager : MonoBehaviour
{
    public static NetworkingRelayManager Instance { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        if (NetworkingRelayManager.Instance != null && NetworkingRelayManager.Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocationHost = await RelayService.Instance.CreateAllocationAsync(NetworkingLobby.Instance.hostLob.MaxPlayers - 1); // need to set max connectiont to Max-1 (for our host)
            string joinCodyRelay = await RelayService.Instance.GetJoinCodeAsync(allocationHost.AllocationId);
            Debug.Log($"JoinCodeRelay: {joinCodyRelay}");

            /// -- > not required to send this data in the latest (if using relay)
            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            //    allocationHost.RelayServer.IpV4,
            //(ushort)allocationHost.RelayServer.Port,
            //allocationHost.AllocationIdBytes,
            //allocationHost.Key,
            //allocationHost.ConnectionData
            //    );
            /// -- > the newer relay version
            RelayServerData relayServerData = new RelayServerData(allocationHost, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            if (NetworkingLobby.Instance) // if we have the lobby handler, send the lobby data for each lobby to take
            {
                NetworkingLobby.Instance.UpdateLobbyData(null, null, null, joinCodyRelay);
                NetworkingLobby.Instance.UpdateLobbyServerData();
            }
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }

    }

    public async void JoinRelay(string _joinCode)
    {
        try
        {
            JoinAllocation allocationJoined = await RelayService.Instance.JoinAllocationAsync(_joinCode);
            Debug.Log($"Joined Relay By Code: {_joinCode}");

            /// -- > not required to send this data in the latest (if using relay)
            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            //    allocationJoined.RelayServer.IpV4,
            //(ushort)allocationJoined.RelayServer.Port,
            //allocationJoined.AllocationIdBytes,
            //allocationJoined.Key,
            //allocationJoined.ConnectionData,
            //allocationJoined.HostConnectionData
            //    );
            /// -- > the newer relay version
            RelayServerData relayServerData = new RelayServerData(allocationJoined, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log($"EXCEPTION: {e}");
        }
    }


}
