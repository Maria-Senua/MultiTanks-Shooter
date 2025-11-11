using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Heal")]
public class HealEffect : PowerUpEffect
{
    public int healAmount = 25;
    public override void Apply(PlayerPowerUpHandler player)
    {
        Debug.Log("here");
        player.ApplyHeal(healAmount);
    }
}
