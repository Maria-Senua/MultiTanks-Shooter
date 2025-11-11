using UnityEngine;
using Unity.Netcode;

public class PowerUp : NetworkBehaviour
{
    [SerializeField]
    private PowerUpEffect effect;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("in trigger");
        gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        if (!IsServer) return;
        PlayerPowerUpHandler player = other.GetComponent<PlayerPowerUpHandler>();
        if (player != null)
        {
            effect.Apply(player);
            NetworkObject.Despawn();
        }
    }
}
