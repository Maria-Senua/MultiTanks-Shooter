using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/Ammo")]
public class AmmoEffect : PowerUpEffect
{
    public int ammoAmount = 5;
    public override void Apply(PlayerPowerUpHandler player)
    {
        Debug.Log("here");
        player.AddAmmo(ammoAmount);
    }
}
