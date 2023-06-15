using UnityEngine;

[CreateAssetMenu(menuName = "Localization/Item")]
public class LocalizationItem : ScriptableObject {
    public string eng;
    public string chs;

    /// <summary>
    /// 得到与当前语言设置对应的语言文本。
    /// </summary>
    /// <returns>与当前语言设置对应的语言文本</returns>
    public string GetText() {
        string language = LocalizationManager.singleton.language;
        return (string) GetType().GetField(language).GetValue(this);
    }
}
