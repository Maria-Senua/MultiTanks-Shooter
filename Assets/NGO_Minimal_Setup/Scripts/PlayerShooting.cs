using Unity.Netcode;
using UnityEngine;

// networked shooting: local input -> server spawns -> everyone sees
public class PlayerShooting : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform shootPoint;        // empty at the barrel tip (Z+)
    [SerializeField] GameObject projectilePrefab; // prefab with NetworkObject + Rigidbody

    [Header("Feel")]
    [SerializeField] float shootForce = 25f;  // forward impulse
    [SerializeField] float upwardForce = 3f;  // little arc so it's juicy
    [SerializeField] float cooldown = 0.5f;   // anti-spam :)

    float lastShot;

    void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        // SPACE to shoot (left click also fine but spec asked Space)
        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastShot >= cooldown)
        {
            lastShot = Time.time;
            ShootServerRpc(shootPoint.position, shootPoint.rotation);
        }
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 pos, Quaternion rot)
    {
        // server is the authority -> it spawns the projectile
        GameObject go = Object.Instantiate(projectilePrefab, pos, rot);

        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            // "forward" here is local Z+ from the shootPoint
            Vector3 dir = rot * Vector3.forward;
            rb.AddForce(dir * shootForce + Vector3.up * upwardForce, ForceMode.Impulse);
        }

        // replicate to clients
        go.GetComponent<NetworkObject>().Spawn(true);
    }
}

