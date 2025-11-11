using UnityEngine;

public abstract class PowerUpEffect : ScriptableObject
{
    public string effectName;
    public GameObject powerUpModel;
    public Sprite icon; // maybe for UI?
    public abstract void Apply(PlayerPowerUpHandler player);
}
