using UnityEngine;

/// <summary>
/// Base class for all power-up effects in the game.  
/// Power-up types inherit from this ScriptableObject and implement their own behavior
/// through the <see cref="Apply(PlayerPowerUpHandler)"/> method.
/// </summary>
public abstract class PowerUpEffect : ScriptableObject
{
    public string effectName;
    public GameObject powerUpModel; // for actual model in future
    public Sprite icon; // for Ui in future
    public abstract void Apply(PlayerPowerUpHandler player);
}
