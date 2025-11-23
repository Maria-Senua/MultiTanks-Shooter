using UnityEngine;
using Unity.Netcode;
using System.Collections;



/// <summary>
/// Handles applying power-ups to player
/// Runs on server
/// </summary>
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


    /// <summary>
    /// Applies temporary speedboost
    /// </summary>
    /// <param name="duration"> How long it lasts in seconds</param>
    /// <param name="multiplier"> The amout to multiply the speed by</param>
    public void ApplySpeedBoost(float duration, float multiplier)
    {
        if (!IsServer) return;
        StartCoroutine(SpeedBoostRoutine(duration, multiplier));
    }

    /// <summary>
    /// Coroutine that handles modifying and restoring player speed
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    private IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        movement.speed.Value *= multiplier;
        yield return new WaitForSeconds(duration);
        movement.speed.Value /= multiplier;
    }

    /// <summary>
    /// Heals player by amount
    /// </summary>
    /// <param name="amount"></param>
    public void ApplyHeal(int amount)
    {
        if (!IsServer) return;
        damage.AddHealth(amount);
        
    }

    /// <summary>
    /// Adds ammo to players ammo count by amount (regular)
    /// </summary>
    /// <param name="amount"></param>
    public void AddAmmo(int amount)
    {
        shooting.ammo += amount;
    }

    /// <summary>
    /// Adds ammo to players ammo count by amount (flame thrower)
    /// </summary>
    /// <param name="amount"></param>
    public void AddFlame(int amount)
    {
        shooting.flameAmmo += amount;
    }

    /// <summary>
    /// Applies a temporary big-bullet effect, increasing bullet size and damage.
    /// </summary>
    /// <param name="duration">How long the power-up lasts in seconds</param>
    /// <param name="scale">The scale to set for bullets during the effect</param>
    public void ApplyBigBullet(float duration, float scale)
    {
        if (!IsServer) return;
        StartCoroutine(BigBulletRoutine(duration, scale));
    }


    /// <summary>
    /// Coroutine that applies the big-bullet modifier and resets it after the duration
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    private IEnumerator BigBulletRoutine(float duration, float scale)
    {
        shooting.bulletScale.Value = scale;
        shooting.damageMultiplier.Value = 3; 
        yield return new WaitForSeconds(duration);
        shooting.bulletScale.Value = 1f;
        shooting.damageMultiplier.Value = 1f;
    }





    private IEnumerator FastChargeRoutine(float duration, float scale)
    {
        shooting.bulletScale.Value = scale;
        shooting.damageMultiplier.Value = 3;
        yield return new WaitForSeconds(duration);
        shooting.bulletScale.Value = 1f;
        shooting.damageMultiplier.Value = 1f;
    }

}
