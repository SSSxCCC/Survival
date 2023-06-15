using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager singleton; // 单例

    /// <summary>
    /// 游戏主音量大小，值在0.0到1.0之间。
    /// </summary>
    public float masterVolume { get { return AudioListener.volume; }
        set {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat(masterVolumeKey, value);
        }
    }

    private const string masterVolumeKey = "masterVolume";

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

    // 根据上次设置调整音量
    private void Initialize() {
        AudioListener.volume = PlayerPrefs.GetFloat(masterVolumeKey, 1);
    }
}
