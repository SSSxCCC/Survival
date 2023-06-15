using UnityEngine;

/// <summary>
/// 此脚本贴到会话窗口上，保证会话窗口一开始是隐藏的。
/// </summary>
public class HideDialog : MonoBehaviour {
    public GameObject dialog;

    private void OnEnable() {
        dialog.SetActive(false);
    }
}
