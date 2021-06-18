using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class ParticleSpawner : NetworkBehaviour
{
    // This script spawns a particle effect. It spawns a particle effect if the owner or
    // other player triggers it
    [SerializeField] private GameObject particlePrefab;

    private void Update()
    {
        // Checking if it is the owner so that the code doesn't run for all the players
        if (!IsOwner) { return; }
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }

        /* RPC - instead of executing code on the client side, we send a message to the server
         * and then the server will run the method. By default only the owner can call for this
         * Simply speakeing, here we are telling the server that we want to spawn some particles.
         * The server recieves it and calls the client RPC to execute code on all the clients.
         */

        SpawnParticleServerRpc();
        Instantiate(particlePrefab, transform.position, transform.rotation);
    }

    /*
     * By default delivery is reliable. If Rpc is faild to be delivered, we send it again
     * and again. But for the things that are not important (like particle effects, SVF, etc)
     * we can simply forget about the rpc if it is lost. If it fails, don't bother sending again.
     * (It is better for the performance)
     */
    [ServerRpc(Delivery = RpcDelivery.Unreliable)] // Attribute
    private void SpawnParticleServerRpc()
    {
        SpawnParticleClientRpc();
    }

    // Server calls this method and it executes on all clients
    [ClientRpc(Delivery = RpcDelivery.Unreliable)] // Attribute
    private void SpawnParticleClientRpc()
    {
        if (IsOwner) { return; }

        // All the clients will run what is specified here
        Instantiate(particlePrefab, transform.position, transform.rotation);

    }
}
