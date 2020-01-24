using UnityEngine;
using UnityEngine.UI;

namespace Offline
{
    /// <summary>
    /// 显示存档路径文本
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class PathText : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Text>().text = SaveUtility.saveFolderPath;
        }
    }
}