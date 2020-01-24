using UnityEngine;
using UnityEngine.SceneManagement;

namespace Offline
{
    /// <summary>
    /// 单机模式游戏内菜单
    /// </summary>
    public class MenuPanel : MonoBehaviour
    {
        // 显示一下所有任务的任务内容
        public void ShowMissions()
        {
            GameManager.singleton.ShowMissionDescriptions();
        }

        // 保存游戏
        public void SaveGame()
        {
            GameManager.singleton.SaveGame();
        }

        // 退出单机模式，回到单机模式菜单
        public void ExitOfflineGame()
        {
            SceneManager.LoadScene("Offline Menu");
        }

        // 隐藏游戏内菜单（必须保证本物体的父物体是In-game Menu此方法才有效）
        public void Hide()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}