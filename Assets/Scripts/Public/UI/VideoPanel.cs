using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 视频选项菜单界面。
/// </summary>
public class VideoPanel : MonoBehaviour {
    public Toggle fullscreenToggle; // 全屏幕开关
    public Toggle vSyncToggle; // 垂直同步开关
    public Dropdown resolutionDropdown; // 分辨率下拉菜单
    public Slider maxFpsSlider; // 最大帧率滑动条

    // 将各个设定界面与实际设置保持一致
    private void Start() {
        // 全屏开关
        fullscreenToggle.isOn = VideoManager.singleton.fullscreen;

        // 垂直同步开关
        vSyncToggle.isOn = VideoManager.singleton.vSync;

        // 分辨率下拉列表
        List<string> optionList = new List<string>();
        foreach (Resolution resolution in Screen.resolutions) {
            optionList.Add(resolution.ToString());
        }
        resolutionDropdown.AddOptions(optionList);
        resolutionDropdown.value = VideoManager.singleton.currentResolutionIndex;

        // 最大帧率滑动条
        maxFpsSlider.value = VideoManager.singleton.maxFps;
    }

    // 当全屏开关改变时调用
    public void OnFullscreenToggleChanged() {
        VideoManager.singleton.fullscreen = fullscreenToggle.isOn;
    }

    // 当垂直同步开关改变时调用
    public void OnVSyncToggleChanged() {
        VideoManager.singleton.vSync = vSyncToggle.isOn;
    }

    // 当分辨率设置改变时调用
    public void OnResolutionDropdownChanged() {
        VideoManager.singleton.currentResolutionIndex = resolutionDropdown.value;
    }

    // 当最大帧率滑动条改变时调用
    public void OnMaxFpsSliderChanged() {
        VideoManager.singleton.maxFps = (int)maxFpsSlider.value;
    }
}
