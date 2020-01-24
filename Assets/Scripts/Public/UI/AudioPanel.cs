using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音频选项界面
/// </summary>
public class AudioPanel : MonoBehaviour
{
    public Slider masterVolumeSlider; // 主音量滑动条

    // 将滑动条状态与音量保持一致
    private void Start()
    {
        masterVolumeSlider.value = AudioManager.singleton.masterVolume;
    }

    // 主音量滑动条改变时调用
    public void OnMasterVolumeChanged()
    {
        AudioManager.singleton.masterVolume = masterVolumeSlider.value;
    }
}
