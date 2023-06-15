using UnityEngine;

/// <summary>
/// 视频管理器，管理分辨率，刷新率等视频相关设定。
/// </summary>
public class VideoManager : MonoBehaviour {
    public static VideoManager singleton; // 单例

    /// <summary>
    /// 是否全屏，等同于Screen.fullScreen属性。
    /// </summary>
    public bool fullscreen { get { return Screen.fullScreen; } set { Screen.fullScreen = value; } }

    /// <summary>
    /// 是否垂直同步。
    /// </summary>
    public bool vSync { get { return QualitySettings.vSyncCount != 0; }
        set {
            QualitySettings.vSyncCount = value ? 1 : 0;
            PlayerPrefs.SetInt(vSyncKey, QualitySettings.vSyncCount);
        }
    }

    int m_CurrentResolutionIndex;
    /// <summary>
    /// 当前分辨率所在Screen.resolutions里面的下标。
    /// </summary>
    public int currentResolutionIndex { get { return m_CurrentResolutionIndex; }
        set {
            if (value < 0 || value >= Screen.resolutions.Length)
                return;

            Resolution resolution = Screen.resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
            m_CurrentResolutionIndex = value;
            PlayerPrefs.SetInt(resolutionIndexKey, value);
        }
    }

    /// <summary>
    /// 最大帧率，当垂直同步开启时无效。
    /// </summary>
    public int maxFps { get { return Application.targetFrameRate; }
        set {
            Application.targetFrameRate = value;
            PlayerPrefs.SetInt(maxFpsKey, value);
        }
    }

    private const string vSyncKey = "vSync";
    private const string resolutionIndexKey = "resolutionIndex";
    private const string maxFpsKey = "maxFps";

    // 单例模式
    private void Awake() {
        if (singleton == null) {
            singleton = this;
        } else if (singleton != this) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    // 初始化，读取上次设定。
    private void Initialize() {
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(vSyncKey, 1);
        m_CurrentResolutionIndex = PlayerPrefs.GetInt(resolutionIndexKey, Screen.resolutions.Length - 1);
        Application.targetFrameRate = PlayerPrefs.GetInt(maxFpsKey, 60);
    }
}