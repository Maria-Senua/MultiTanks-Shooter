using System;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI damageText;

    public float health = 100;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText.SetText(health.ToString());
        HideDamageText();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            float damageTaken = other.gameObject.GetComponent<Projectile>().damage;
            health -= damageTaken;
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
        
    }
}
