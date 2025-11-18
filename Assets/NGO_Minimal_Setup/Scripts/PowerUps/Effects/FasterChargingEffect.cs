using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "PowerUps/Effects/Fast Charge")]
public class FasterChargingEffect : PowerUpEffect
{
    public float duration;
    public float bulletScale;
    public override void Apply(PlayerPowerUpHandler player)
    {
        player.ApplyBigBullet(duration, bulletScale);
    }
}
