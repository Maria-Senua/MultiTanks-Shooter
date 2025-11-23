using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;


/// <summary>
/// Handles spawning of power-ups
/// Only the server runs the spawning logic since power-ups are networked objects
/// </summary>
public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject[] powerUpPrefabs; // array of spawnable power-ups

    public Spawnpoint[] spawnPoints; // all spawn point

    public float spawnInterval; // time between spawns in seconds

    public override void OnNetworkSpawn()
    {
        if (IsServer) 
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    /// <summary>
    /// Repeatedly spawns power-ups at fixed intervals.
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
           
            SpawnPowerUp();
        }
    }


    /// <summary>
    /// Chooses a random free spawn point and a random power-up prefab,
    /// instantiates it, and marks the spawn point as occupied.
    /// </summary>
    private void SpawnPowerUp()
    {
        List<Spawnpoint> freeSpawnpoints = spawnPoints.Where(p => p.IsFree.Value).ToList();

        if (freeSpawnpoints.Count == 0)
            return;

        Spawnpoint point = freeSpawnpoints[UnityEngine.Random.Range(0, freeSpawnpoints.Count)];
        GameObject prefabToSpawn = powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Length)];

        GameObject powerUp = Instantiate(prefabToSpawn, point.transform.position, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();

        point.IsFree.Value = false;

        powerUp.GetComponent<PowerUp>().Init(point);
    }

}
