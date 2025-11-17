using UnityEngine;
using TMPro;

public class WidgetController : MonoBehaviour
{
    public GameObject widget;
    public TMP_Text btnLabel;

    public void HideOrShowWidget()
    {
        bool newState = !widget.activeSelf;
        widget.SetActive(newState);
        btnLabel.text = newState ? "Hide" : "Show";
    }
}
