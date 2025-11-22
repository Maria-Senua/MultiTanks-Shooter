using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class FlameDamage : NetworkBehaviour
{
    private ParticleSystem ps;
    public float damage = 2f;
    private List<Collider> playerColliders = new List<Collider>();
    [HideInInspector] public GameObject owner;


    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        
    }
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            // Only the server handles damage
            enabled = false;
            return;
        }

        UpdatePlayerColliders();
    }
    public void UpdatePlayerColliders()
    {
        playerColliders.Clear();

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var col = player.GetComponent<Collider>();
            if (col != null) playerColliders.Add(col);
        }

        var trigger = ps.trigger;
        trigger.SetCollider(0, null); // clear first
        for (int i = 0; i < playerColliders.Count; i++)
        {
            trigger.AddCollider(playerColliders[i]);
        }
    }
    

    // Call again whenever a new player spawns
    public void AddAllPlayerColliders()
    {
        ParticleSystem.TriggerModule trigger = ps.trigger;

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var col = player.GetComponent<Collider>();
            if (col != null)
            {
                trigger.AddCollider(col);
            }
        }
    }

    private void OnParticleTrigger()
    {
        if (!IsServer) return;

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        int count = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < count; i++)
        {
            Collider[] hits = Physics.OverlapSphere(enter[i].position, 10.2f);

            foreach (var hit in hits)
            {
                if (hit.gameObject == owner) continue;
                var dmg = hit.GetComponent<TakeDamage>();
                if (dmg != null)
                {
                    dmg.health.Value -= damage;
                }
            }
        }
    }
}
