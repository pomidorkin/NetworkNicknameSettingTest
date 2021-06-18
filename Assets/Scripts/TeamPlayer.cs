using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Connection;
using MLAPI.NetworkVariable;
using System;

public class TeamPlayer : NetworkBehaviour
{
    // [SerializeField] private Renderer teamColourRenderer;
    [SerializeField] private Renderer teamColourRenderer;
    [SerializeField] private Color[] teamColors;

    /* Normal byte or int is simply updated on the server side, no calls,
     * no sync, clients will not be aware of this change. So we need to sync this data
     * among all the clients, including those who joined late. For this we use
     * NetworkVariable. NetworkVariable allows us to sync data between the server
     * and the clients. 
     */
    private NetworkVariableByte teamIndex = new NetworkVariableByte();

    [ServerRpc]
    public void SetTeamServerRpc(byte newTeamIndex)
    {
        if(newTeamIndex > 3) { return; }
        teamIndex.Value = newTeamIndex;
    }

    private void OnEnable()
    {

        // When the value is changed the subscribed method will be triggered
        teamIndex.OnValueChanged += OnTeamChanged;
    }

   
    private void OnDisable()
    {
        teamIndex.OnValueChanged -= OnTeamChanged;
    }

    private void OnTeamChanged(byte oldTeamIndex, byte newTeamIndex)
    {
        // If it is a server, it doesn't need to render anything
        if (!IsClient) { return; }

        // Updating the renderer
        teamColourRenderer.material.SetColor("_BaseColor", teamColors[newTeamIndex]);
    }
}
