using Unity.Netcode;
using UnityEngine;

// tiny networked projectile: exists, hits, then politely leaves
public class Projectile : NetworkBehaviour
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] GameObject explosion;

    void Start()
    {
        if (IsServer)
            Invoke(nameof(Despawn), lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        //add explosion particles
        var contactPoint = collision.contacts[0].point;
        var boom = Instantiate(explosion, contactPoint, Quaternion.identity);
        Debug.Log("BOOM " + explosion.activeInHierarchy);
        Destroy(boom, 2f);
        //if (IsServer) Despawn();
        if (IsServer)
            Invoke(nameof(Despawn), 0.05f);
    }

    void Despawn()
    {
        if (NetworkObject && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}
