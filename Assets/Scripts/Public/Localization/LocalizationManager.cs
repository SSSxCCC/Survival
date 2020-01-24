using UnityEngine;

/// <summary>
/// 本地化管理器，主要用来设置游戏语言。
/// </summary>
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager singleton; // 单例
    
    string m_Language;
    /// <summary>
    /// 当前语言，其值可能为{eng, chs}。
    /// 若值发生改变，则触发语言变更事件，保存语言设置记录。
    /// </summary>
    public string language { get { return m_Language; }
        set {
            if (string.Equals(m_Language, value)) return; // 新语言与旧的相同，什么都不做。
            m_Language = value;
            if (LanguageChange != null) LanguageChange(); // 触发语言变更事件
            PlayerPrefs.SetString(languageKey, m_Language); // 保存语言设置记录
            PlayerPrefs.Save();
        }
    }

    public event LanguageChangeEventHandler LanguageChange; // 语言变更事件
    public delegate void LanguageChangeEventHandler();

    private const string languageKey = "language";

    // 单例模式
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    // 根据上次的设置或系统语言来调整游戏语言
    private void Initialize()
    {
        string savedLanguage = PlayerPrefs.GetString(languageKey); // 得到语言设置记录

        if (string.IsNullOrEmpty(savedLanguage)) // 没有语言设置记录
        {
            // 根据系统语言设置语言
            if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
                savedLanguage = "chs";
            else
                savedLanguage = "eng";

            PlayerPrefs.SetString(languageKey, savedLanguage);
            PlayerPrefs.Save();
        }

        m_Language = savedLanguage;
    }
}