using UnityEngine;
using Unity.Netcode;

public class Spawnpoint : NetworkBehaviour
{
    public NetworkVariable<bool> IsFree = new(true);

    private void OnDrawGizmos()
    {
        Gizmos.color = IsFree.Value ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
