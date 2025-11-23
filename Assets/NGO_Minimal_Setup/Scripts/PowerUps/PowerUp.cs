using UnityEngine;
using Unity.Netcode;
using System.Drawing;


/// <summary>
/// Represents a networked power-up object that applies a specific effect
/// to the player when they collide with it. Automatically despawns after use.
/// </summary>
public class PowerUp : NetworkBehaviour
{
    [SerializeField]
    private PowerUpEffect effect;

    Spawnpoint spawnpoint;

    public  void Init(Spawnpoint sp)
    {
       spawnpoint = sp;
    }


    /// <summary>
    /// Trigger callback that checks if the colliding object is a player.
    /// If so, applies the power-up's effect and despawns the object on the server
    /// </summary>
    /// <param name="other">The collider that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        PlayerPowerUpHandler player = other.GetComponent<PlayerPowerUpHandler>();
        if (player != null)
        {
            effect.Apply(player);
            NetworkObject.Despawn();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && spawnpoint != null)
            spawnpoint.IsFree.Value = true;
    }
}
