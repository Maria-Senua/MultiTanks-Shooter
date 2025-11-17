using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject[] powerUpPrefabs;

    public Spawnpoint[] spawnPoints;

    public float spawnInterval;

    //private readonly Dictionary<Transform, GameObject> activePowerUps = new Dictionary<Transform, GameObject>();

    public override void OnNetworkSpawn()
    {
        if (IsServer) // no not for multiplayer
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
           
            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        //SpawnRandom();
        List<Spawnpoint> freeSpawnpoints = spawnPoints.Where(p => p.IsFree.Value).ToList();

        if (freeSpawnpoints.Count == 0)
            return;

        Spawnpoint point = freeSpawnpoints[UnityEngine.Random.Range(0, freeSpawnpoints.Count)];
        GameObject prefabToSpawn = powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Length)];

        GameObject powerUp = Instantiate(prefabToSpawn, point.transform.position, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();

        //spawnPoints.Where(p => p.transform.position == point.transform.position).First().IsFree.Value = false;
        point.IsFree.Value = false;

        powerUp.GetComponent<PowerUp>().Init(point);
    }

    //private void SpawnRandom()
    //{
    //    int prefabIndex = UnityEngine.Random.Range(0, powerUpPrefabs.Length);
    //    GameObject prefabToSpawn = powerUpPrefabs[prefabIndex];

    //    List<Transform> tempSpawnPoints = new List<Transform>();
    //    foreach (Transform t in spawnPoints)
    //    {
    //        if(!activePowerUps.ContainsKey(t) || activePowerUps[t] == null)
    //        {
    //            tempSpawnPoints.Add(t);
    //        }
    //    }
    //    if (tempSpawnPoints.Count == 0) return;

    //    int spawnIndex = UnityEngine.Random.Range(0, tempSpawnPoints.Count);
    //    //int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length); 
    //    //Transform spawnPoint = spawnPoints[spawnIndex];
    //    Transform spawnPoint = tempSpawnPoints[spawnIndex];
    //    GameObject powerUp = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
    //    powerUp.GetComponent<NetworkObject>().Spawn(true);


    //    activePowerUps[spawnPoint] = powerUp;
    //    tempSpawnPoints.Clear();    
    //    Debug.Log(transform.name);
    //}
}
