using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    
    private TextMeshProUGUI ammoText;
    private PlayerShooting playerShoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoText = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerShoot == null)
        {
            foreach (var obj in FindObjectsOfType<PlayerShooting>())
            {
                if (obj.IsOwner)   
                {
                    playerShoot = obj;
                    break;
                }
            }

            if (playerShoot == null)
            {
                ammoText.text = "blabls";
                return;
            }
        }

        if (playerShoot.currentWeapon == WeaponType.Projectile)
            ammoText.text = "Ammo: " + playerShoot.ammo.ToString();
        else
            ammoText.text = "Flame: " + playerShoot.flameAmmo.ToString();
    }

}
