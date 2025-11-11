using Unity.Netcode;
using UnityEngine;

// tiny networked projectile: exists, hits, then politely leaves
// server decides the hit; clients spawn the explosion once via RPC
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] GameObject explosion; // explosion prefab
    public float damage = 10f;

    Rigidbody rb;
    Collider[] cols;
    bool impacted = false; // single-hit guard

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cols = GetComponentsInChildren<Collider>(false);
    }

    public override void OnNetworkSpawn()
    {
        if (rb) rb.isKinematic = !IsServer; // server simulates; clients follow via NetworkTransform
        if (IsServer && lifeTime > 0f) Invoke(nameof(Despawn), lifeTime);
    }

    void OnCollisionEnter(Collision c)
    {
        if (!IsServer) return;
        if (impacted) return;
        impacted = true;

        // stop further contacts this frame
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.detectCollisions = false;
            rb.isKinematic = true;
        }
        if (cols != null) foreach (var col in cols) if (col) col.enabled = false;

        Vector3 hit = (c.contactCount > 0) ? c.GetContact(0).point : transform.position;

        if (explosion) SpawnExplosionClientRpc(hit);

        Invoke(nameof(Despawn), 0.02f); // let the RPC fly first
    }

    [ClientRpc]
    void SpawnExplosionClientRpc(Vector3 pos)
    {
        if (!explosion) return;
        var fx = Instantiate(explosion, pos, Quaternion.identity);
        Destroy(fx, 2f); // safety in case the particle doesn't auto-destroy
    }

    void Despawn()
    {
        if (NetworkObject && NetworkObject.IsSpawned) NetworkObject.Despawn();
        else Destroy(gameObject);
    }
}
