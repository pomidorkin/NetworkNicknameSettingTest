using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using System;

public class PlayerOverheadDisplay : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText;

    private NetworkVariableString displayName = new NetworkVariableString();

    public override void NetworkStart()
    {
        if (!IsServer) { return; }
        // Whoever this player is, go get their player data
        PlayerData? playerData = PasswordNetworkManager.GetPlayerData(OwnerClientId);

        if (playerData.HasValue)
        {
            displayName.Value = playerData.Value.PlayerName;
        }
    }

    private void OnEnable()
    {
        displayName.OnValueChanged += HandleDisplayValueChanged;
    }

    private void HandleDisplayValueChanged(string oldDisplayName, string newDisplayName)
    {
        displayNameText.text = newDisplayName;
    }

    private void OnDisable()
    {
        displayName.OnValueChanged -= HandleDisplayValueChanged;
    }
}
