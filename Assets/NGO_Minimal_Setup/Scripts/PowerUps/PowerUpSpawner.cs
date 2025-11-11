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

    private readonly Dictionary<Transform, GameObject> activePowerUps = new Dictionary<Transform, GameObject>();

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

        List<Transform> tempSpawnPoints = new List<Transform>();
        foreach (Transform t in spawnPoints)
        {
            if(!activePowerUps.ContainsKey(t) || activePowerUps[t] == null)
            {
                tempSpawnPoints.Add(t);
            }
        }
        if (tempSpawnPoints.Count == 0) return;

        int spawnIndex = UnityEngine.Random.Range(0, tempSpawnPoints.Count);
        //int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length); 
        //Transform spawnPoint = spawnPoints[spawnIndex];
        Transform spawnPoint = tempSpawnPoints[spawnIndex];
        GameObject powerUp = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        powerUp.GetComponent<NetworkObject>().Spawn(true);

        
        activePowerUps[spawnPoint] = powerUp;
        tempSpawnPoints.Clear();    
        Debug.Log(transform.name);
    }
}
