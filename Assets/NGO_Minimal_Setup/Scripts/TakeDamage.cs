using JetBrains.Annotations;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;
using Cysharp.Threading.Tasks;

public class TakeDamage : NetworkBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private TMPro.TextMeshProUGUI damageText;
    [SerializeField] private TMPro.TextMeshProUGUI gameOverText;

    public NetworkVariable<float> health = new NetworkVariable<float>(100);
    //public float health = 100;
    private bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
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
           // ChangeHealthClientRpc();
        }
    }



    void HideDamageText()
    {
        damageText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) return;
        
        ChangeHealth();
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
        if (IsOwner && health.Value <= 0 && !isDead)
        {
            //add Game over or sth
            isDead = true;
            Debug.Log("LeaveSessionAfterDeath get ready");
            gameOverText.gameObject.SetActive(true);
            LeaveSessionAfterDeath().Forget();
        }     
    }

    private async UniTaskVoid LeaveSessionAfterDeath()
    {
        
        await UniTask.Delay(TimeSpan.FromSeconds(2));

        try
        {
            if (SessionManager.Instance != null)
            {
                await SessionManager.Instance.LeaveSession();
            }
            else
            {
                Debug.LogWarning("SessionManager.Instance was null.");
            }

            if (!IsServer)
            {
                if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
                {
                    NetworkManager.Singleton.Shutdown();

                }
            }
           

         
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while leaving session: {e}");
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

    //[ClientRpc]
    void ChangeHealth()
    {
      
        healthText.SetText(health.Value.ToString());
        Invoke(nameof(HideDamageText), 2f);
    
    }


}
