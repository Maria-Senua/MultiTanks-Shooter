using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/Effects/FireAmmo")]
public class FireAmmoEffect : PowerUpEffect
{
    public int ammoAmount = 5;
    public override void Apply(PlayerPowerUpHandler player)
    {
        Debug.Log("here");
        player.AddFlame(ammoAmount);
    }
}
