using Unity.Netcode;
using UnityEngine;

// tiny networked projectile: exists, hits, then politely leaves
public class Projectile : NetworkBehaviour
{
    [SerializeField] float lifeTime = 5f;

    void Start()
    {
        if (IsServer)
            Invoke(nameof(Despawn), lifeTime);
    }

    void OnCollisionEnter(Collision _)
    {
        if (IsServer) Despawn();
    }

    void Despawn()
    {
        if (NetworkObject && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}
