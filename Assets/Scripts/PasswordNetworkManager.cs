using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System;

public class PasswordNetworkManager : MonoBehaviour
{

    /*
     * Passwor Protected Lobby example
     * "Connection Approval" on the NetworkManager component shold be checked
     */

    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject passwordEntryUI;
    [SerializeField] private GameObject leaveButton;

    // Storing client data. ulong = client id
    private static Dictionary<ulong, PlayerData> clientData;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        // It is important to unsubscribe from the publisher when the object is destroyed
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void Host()
    {
        clientData = new Dictionary<ulong, PlayerData>();
        // Adding client data for the host
        clientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(nameInputField.text);

        // Subcribing to the callback to implement our logic for the connection approval check
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(new Vector3(-2f, 0f, 0f), Quaternion.Euler(0f, 135f, 0f));
    }


    public void Client()
    {
        // Converting our client data into a json string and then converting the
        // string into a byte array to be sent by the network
        var payload = JsonUtility.ToJson(new ConnectionPayload()
        {
            password = passwordInputField.text,
            playerName = nameInputField.text

        });

        byte[] payloadBytes = Encoding.ASCII.GetBytes(payload);

        // This is where we set connection date to send the password as a client
        // Because we cannot send data like stings through the network, we need to
        // convert the payload to the byte array
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            // Unsubcribe from the approval check
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        } else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }

        passwordEntryUI.SetActive(true);
        leaveButton.SetActive(false);
    }

    public static PlayerData? GetPlayerData(ulong clientId)
    {
        // We are saying "Give me the data for this client id"
        if (clientData.TryGetValue(clientId, out PlayerData playerData))
        {
            return playerData;
        }

        return null;
    }

    // OnClientConnected doesn't get called for the host when the host connects, so it need to be done manually
    private void HandleServerStarted()
    {
        // If we are running as a host
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnect(NetworkManager.Singleton.LocalClientId);
        }
    }

    // Called on the server each time when a client is connected.
    // It is calse called for the client side when the themselves connect
    private void HandleClientConnect(ulong clientId)
    {
        // If it's us, whe make the UI unactive. When we connect, turn this on and off
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            clientData.Remove(clientId);
        }


        // When we disconnect, turn this on and off
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            passwordEntryUI.SetActive(true);
            leaveButton.SetActive(false);
        }
    }


    // In this method we write our custom logic.
    // connectionData will contain password or whatever we decide to send.
    // Note: This code doesn't get called for the host.
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        // connectionData is a byteArray and to check the password we need to convert it into a string
        string payload = Encoding.ASCII.GetString(connectionData);

        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        bool approvedConnection = connectionPayload.password == passwordInputField.text;

        // This ugly spawning implementation is just for the demonstration
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (approvedConnection)
        {
            // Counting connected clients and assigning them spawn coords
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPos = new Vector3(0f, 0f, 0f);
                    spawnRot = Quaternion.Euler(0f, 180f, 0f);
                    break;

                case 2:
                    spawnPos = new Vector3(2f, 0f, 0f);
                    spawnRot = Quaternion.Euler(0f, 225f, 0f);
                    break;
            }

            // Setting a name to the client
            clientData[clientId] = new PlayerData(connectionPayload.playerName);
        }

        callback(true, null, approvedConnection, null, null);
    }

}
