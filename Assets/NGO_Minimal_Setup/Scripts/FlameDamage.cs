using System;
using UnityEngine;
using System.Collections.Generic;

public class FlameDamage : MonoBehaviour
{
    private ParticleSystem ps;
    public float damage = 2f;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        AddAllPlayerColliders();
    }

    private void Update()
    {
        AddAllPlayerColliders();
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
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        int count = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < count; i++)
        {
            Collider[] hits = Physics.OverlapSphere(enter[i].position, 0.2f);

            foreach (var hit in hits)
            {
                var dmg = hit.GetComponent<TakeDamage>();
                if (dmg != null)
                {
                    dmg.health.Value -= damage;
                    Debug.Log("Flame hit player! HP = " + dmg.health.Value);
                }
            }
        }
    }
}
