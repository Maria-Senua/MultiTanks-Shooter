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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gameover") )
        {
            if (IsOwner && !isDead)
            {
                isDead = true;
                Debug.Log("Player died");

                gameOverText.gameObject.SetActive(true);

                if (IsHost)
                {
                    Debug.Log("Host died → disabling host gameplay but keeping server alive");
                    DisableHostGameplayClientRpc();
                    return; 
                }
                LeaveSessionAfterDeath().Forget();
            }
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
            isDead = true;
            Debug.Log("Player died");

            gameOverText.gameObject.SetActive(true);

            if (IsHost)
            {
                Debug.Log("Host died → disabling host gameplay but keeping server alive");
                DisableHostGameplayClientRpc();
                return; 
            }
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
    
    [ClientRpc]
    private void DisableHostGameplayClientRpc()
    {
        var movement = GetComponent<TankMovement>();
        if (movement) movement.enabled = false;

        var shooting = GetComponent<PlayerShooting>();
        if (shooting) shooting.enabled = false;

        var cc = GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider) collider.enabled = false;
        foreach (var childCollider in GetComponentsInChildren<Collider>())
            childCollider.enabled = false;
        damageText.gameObject.SetActive(false);
        healthText.gameObject.SetActive(false);

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        Debug.Log("Host died but server stays alive");
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
