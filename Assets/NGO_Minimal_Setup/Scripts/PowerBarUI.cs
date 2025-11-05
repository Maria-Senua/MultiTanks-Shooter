using UnityEngine;
using UnityEngine.UI;

// Simple singleton for power bar UI
public class PowerBarUI : MonoBehaviour
{
    public static PowerBarUI Instance;
    [SerializeField] private Image fillImage;

    void Awake()
    {
        Instance = this;
    }

    // value between 0 and 1
    public void SetCharge(float value)
    {
        if (fillImage)
            fillImage.fillAmount = Mathf.Clamp01(value);
    }
}

