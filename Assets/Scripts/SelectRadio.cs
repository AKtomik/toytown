using UnityEngine;
using UnityEngine.UI;

public class SelectRadio : MonoBehaviour
{
    public Toggle toggle;
    public Image target;
    public Color onColor;
    public Color offColor;

    void Awake()
    {
        toggle.onValueChanged.AddListener(UpdateColor);
        UpdateColor(toggle.isOn);
    }

    void UpdateColor(bool isOn)
    {
        target.color = isOn ? onColor : offColor;
    }
}
