using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class Ball : NetworkBehaviour
{
    [SerializeField] Renderer ballRenderer;

    // Network variable syncs the value
    private NetworkVariableColor ballColour = new NetworkVariableColor();

    public override void NetworkStart()
    {
        // Make sure we are server
        if (!IsServer) { return; }

        // Generating a random color for the ball
        ballColour.Value = Random.ColorHSV();
    }

    private void Update()
    {
        if (!IsOwner) { return; }
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        DestroyBallServerRpc();
    }

    [ServerRpc]
    private void DestroyBallServerRpc()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        ballColour.OnValueChanged += OnBallColorChanged;
    }

    private void OnDisable()
    {
        ballColour.OnValueChanged -= OnBallColorChanged;
    }

    private void OnBallColorChanged(Color oldBallColor, Color newBallColor)
    {
        // Only clients need to update the renderer
        if (!IsClient) { return; }

        ballRenderer.material.SetColor("_BaseColor", newBallColor);
    }


}
