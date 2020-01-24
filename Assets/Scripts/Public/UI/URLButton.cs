using UnityEngine;

public class URLButton : MonoBehaviour
{
    /// <summary>
    /// 使用系统默认浏览器打开网页。
    /// </summary>
    /// <param name="url">网址</param>
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
