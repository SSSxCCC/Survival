using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MaxFpsValueText : MonoBehaviour
{
    public Slider maxFpsSlider;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void OnMaxFpsChanged()
    {
        text.text = maxFpsSlider.value.ToString();
    }
}
