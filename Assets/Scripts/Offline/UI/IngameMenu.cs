using UnityEngine;

namespace Offline
{
    /// <summary>
    /// 单机模式游戏内选项菜单。
    /// </summary>
    public class IngameMenu : MonoBehaviour
    {
        // 打开或关闭游戏内选项菜单界面
        public void OpenOrClose()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        // 菜单显示时游戏暂停
        private void OnEnable()
        {
            Time.timeScale = 0;
        }
        private void OnDisable()
        {
            Time.timeScale = 1;
        }
    }
}