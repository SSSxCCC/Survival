using UnityEngine;

public class LanguagePanel : MonoBehaviour
{
    // 改变游戏语言
    public void ChangeLanguage(string language)
    {
        LocalizationManager.singleton.language = language;
    }
}