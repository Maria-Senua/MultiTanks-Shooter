using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerPowerUpHandler : NetworkBehaviour
{
    private TankMovement movement;
    private TakeDamage damage;
    private PlayerShooting shooting;

    private void Awake()
    {
        movement = GetComponent<TankMovement>();
        damage = GetComponent<TakeDamage>();    
        shooting = GetComponent<PlayerShooting>();  
    }

    public void ApplySpeedBoost(float duration, float multiplier)
    {
        if (!IsServer) return;
        StartCoroutine(SpeedBoostRoutine(duration, multiplier));
    }

    private IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        //movement.moveSpeed *= multiplier;
        movement.speed.Value *= multiplier;
        yield return new WaitForSeconds(duration);
        movement.speed.Value /= multiplier;
        //movement.moveSpeed /= multiplier;
    }

    public void ApplyHeal(int amount)
    {
        if (!IsServer) return;
        damage.AddHealth(amount);
        
    }

    public void ApplyBigBullet(float duration, float scale)
    {
        if (!IsServer) return;
        StartCoroutine(BigBulletRoutine(duration, scale));
    }

    private IEnumerator BigBulletRoutine(float duration, float scale)
    {
        shooting.bulletScale.Value = scale;
        yield return new WaitForSeconds(duration);
        shooting.bulletScale.Value = 1f;
    }

}
