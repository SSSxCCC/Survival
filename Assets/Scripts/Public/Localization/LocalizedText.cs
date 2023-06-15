using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour {
    public LocalizationItem localizationItem;

    private Text uiText;

    private void Start() {
        uiText = GetComponent<Text>();

        uiText.text = localizationItem.GetText();

        LocalizationManager.singleton.LanguageChange += OnChangeLanguage;
    }

    private void OnDestroy() {
        LocalizationManager.singleton.LanguageChange -= OnChangeLanguage;
    }

    private void OnChangeLanguage() {
        uiText.text = localizationItem.GetText();
    }
}
