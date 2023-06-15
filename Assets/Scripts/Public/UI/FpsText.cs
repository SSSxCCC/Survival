using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsText : MonoBehaviour {
    public LocalizationItem fpsItem;

    private const float interval = 1f; // 计算FPS时间间隔

    private float timePassed = 0f; // 已经统计的时间
    private int frameCount = 0; // 统计帧数

    private Text fpsText; // 显示FPS的标签

    private void Start() {
        fpsText = GetComponent<Text>();
    }

    private void Update() {
        // 统计时间和帧数
        timePassed += Time.unscaledDeltaTime;
        frameCount++;

        // 计算帧率
        if (timePassed >= interval) {
            float fps = frameCount / timePassed;
            fpsText.text = fpsItem.GetText() + ": " + fps.ToString("f2");
            timePassed = 0;
            frameCount = 0;
        }
    }
}
