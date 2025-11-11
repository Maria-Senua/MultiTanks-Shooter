using System;
using UnityEngine;
using Unity.Netcode;

public class DustMaker : NetworkBehaviour
{
    [SerializeField] private GameObject dust;

    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;
        if (other.gameObject.tag == "Destructible")
        {
            var contactPoint = other.contacts[0].point;
            SpawnDustClientRpc(contactPoint);
        }
    }
    
    [ClientRpc]
    private void SpawnDustClientRpc(Vector3 position)
    {
        var newDust = Instantiate(dust, position, Quaternion.identity);
        Destroy(newDust, 10f);
    }
}
