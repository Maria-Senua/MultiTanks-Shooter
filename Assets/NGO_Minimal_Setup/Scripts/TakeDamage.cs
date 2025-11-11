using JetBrains.Annotations;
using System;
using Unity.Netcode;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI damageText;

    public NetworkVariable<float> health = new NetworkVariable<float>(100);
    //public float health = 100;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText.SetText(health.Value.ToString());
        HideDamageText();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            float damageTaken = other.gameObject.GetComponent<Projectile>().damage;
            health.Value -= damageTaken;
            damageText.SetText("-" + damageTaken.ToString());
            healthText.SetText(health.ToString());
            Invoke(nameof(HideDamageText), 2f);
        }
    }

    void HideDamageText()
    {
        damageText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (health.Value >= 70)
        {
            healthText.color = new Color32(86, 188, 58, 255);
        }
        else if (health.Value < 70 && health.Value >= 40)
        {
            healthText.color = new Color32(231, 150, 12, 255);
        }
        else if (health.Value < 40)
        {
            healthText.color = new Color32(224, 20, 20, 255);
        }
        if (health.Value <= 0)
        {
            //add Game over or sth
            
        }     
    }

    public void AddHealth(int amount)
    {
        if (health.Value < 100)
        {
            health.Value += amount;
            healthText.SetText(health.Value.ToString());
        }  
    }
}
