using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject[] powerUpPrefabs;

    public Transform[] spawnPoints;

    public float spawnInterval;

    private readonly Dictionary<Transform, Boolean> activePowerUps = new Dictionary<Transform, Boolean>();

    private void Start()
    {
        if (!IsServer) // no not for multiplayer
        {
            Debug.Log("HEEEERE");
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandom();
        }
    }

    private void SpawnRandom()
    {
        int prefabIndex = UnityEngine.Random.Range(0, powerUpPrefabs.Length);
        GameObject prefabToSpawn = powerUpPrefabs[prefabIndex];


        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject powerUp = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        powerUp.GetComponent<NetworkObject>().Spawn(true);

        Debug.Log(transform.name);
    }
}
