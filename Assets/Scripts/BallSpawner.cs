using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BallSpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject ballPrefab;
    private Camera mainCamera;


    // Start is called before the first frame update
    void Start()
    {
        // We can camera to determine where we click on the scene
        mainCamera = Camera.main;
    }


    private void Update()
    {
        if (!IsOwner) { return; }
        if (!Input.GetMouseButtonDown(0)) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        SpawnBallServerRpc(hit.point);
    }

    [ServerRpc]
    private void SpawnBallServerRpc(Vector3 spawnPos)
    {
        // Spawning ball on the server side
        NetworkObject ballInstnce = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        // We to spawn the ball and give the ownership to the player who spawned the ball
        ballInstnce.SpawnWithOwnership(OwnerClientId);
    }

}
