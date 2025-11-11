using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Speed Boost")]
public class SpeedBoostEffect : PowerUpEffect
{
    public float duration = 10f;
    public float multiplier = 5f;
    public override void Apply(PlayerPowerUpHandler player)
    {
        player.ApplySpeedBoost(duration, multiplier);
    }
}
