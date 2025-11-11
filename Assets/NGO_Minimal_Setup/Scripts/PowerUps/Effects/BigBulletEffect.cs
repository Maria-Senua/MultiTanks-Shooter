using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "PowerUps/Effects/Big Bullet")]
public class BigBulletEffect : PowerUpEffect
{
    public float duration;
    public float bulletScale;
    public override void Apply(PlayerPowerUpHandler player)
    {
        player.ApplyBigBullet(duration, bulletScale);
    }
}
