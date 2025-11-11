using System;
using UnityEngine;
using Unity.Netcode;

public class TerraHoles : NetworkBehaviour
{
    [SerializeField] private Terrain t;

    [SerializeField] private int holeWidth = 10;
    [SerializeField] private int holeHeight = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        TerrainData terrainData = t.terrainData;
        //if (IsServer)
        //{
            int res = terrainData.holesResolution;
            bool[,] fullHoles = new bool[res, res];

            for (int x = 0; x < res; x++)
            {
                for (int z = 0; z < res; z++)
                {
                    fullHoles[x, z] = true;
                }
            }

            terrainData.SetHoles(0, 0, fullHoles);   
        //}
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;
        if (other.gameObject.tag == "Bullet")
        {
            Vector3 contactPoint = other.contacts[0].point;

            CreateHole(contactPoint);
            CreateHoleClientRpc(contactPoint);
        }
    }
    [ClientRpc]
    private void CreateHoleClientRpc(Vector3 contactPoint)
    {
        if (IsServer) return; 
        CreateHole(contactPoint);
    }
    void CreateHole(Vector3 contactPoint)
    {
        TerrainData terrainData = t.terrainData;
        Vector3 terrainPos = t.transform.position;

        float relativeX = (contactPoint.x - terrainPos.x) / terrainData.size.x;
        float relativeZ = (contactPoint.z - terrainPos.z) / terrainData.size.z;

        int holeMapX = (int)(relativeX * terrainData.holesResolution);
        int holeMapZ = (int)(relativeZ * terrainData.holesResolution);

        int offsetX = holeWidth / 2;
        int offsetZ = holeHeight / 2;

        int startX = Mathf.Clamp(holeMapX - offsetX, 0, terrainData.holesResolution - holeWidth);
        int startZ = Mathf.Clamp(holeMapZ - offsetZ, 0, terrainData.holesResolution - holeHeight);

        bool[,] holes = terrainData.GetHoles(startX, startZ, holeWidth, holeHeight);

        Vector2 center = new Vector2(holeWidth / 2f, holeHeight / 2f);
        float radius = holeWidth / 2f;

        for (int x = 0; x < holeWidth; x++)
        {
            for (int z = 0; z < holeHeight; z++)
            {
                float dist = Vector2.Distance(new Vector2(x, z), center);
                holes[z, x] = dist > radius;
            }
        }

        terrainData.SetHoles(startX, startZ, holes);
    }
}
