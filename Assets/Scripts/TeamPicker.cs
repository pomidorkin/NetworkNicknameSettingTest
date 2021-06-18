using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;

public class TeamPicker : MonoBehaviour
{
    public void SelectTeam(int teamIndex)
    {
        // Getting our own player prefab from the network manager
        // First, we get the player id (LocalClientId)
        ulong localClientId =  NetworkManager.Singleton.LocalClientId;

        // We are telling the network manager "please, get us the player with this id"
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }

        // Getting the player objects
        if(!networkClient.PlayerObject.TryGetComponent<TeamPlayer>(out TeamPlayer teamPlayer))
        {
            return;
        }

        // Now when we got our own TeamPlayer component, we want to send a server RPC
        // (executed on the server side)
        teamPlayer.SetTeamServerRpc((byte)teamIndex);
    }
}
