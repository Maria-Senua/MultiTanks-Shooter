using UnityEngine;
using Unity.Netcode;
using System.Drawing;

public class PowerUp : NetworkBehaviour
{
    [SerializeField]
    private PowerUpEffect effect;

    Spawnpoint spawnpoint;

    public  void Init(Spawnpoint sp)
    {
       spawnpoint = sp;
    }

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
