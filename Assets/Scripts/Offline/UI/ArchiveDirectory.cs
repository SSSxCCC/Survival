using UnityEngine;

namespace Offline
{
    /// <summary>
    /// 存档目录按钮
    /// </summary>
    public class ArchiveDirectory : MonoBehaviour
    {
        // 打开存档目录
        public void Open()
        {
            System.Diagnostics.Process.Start(SaveUtility.saveFolderPath);
        }
    }
}