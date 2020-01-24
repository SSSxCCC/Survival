using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Offline
{
    public class ArchiveWindow : MonoBehaviour
    {
        [Range(1, 3)] public int saveID;

        public GameObject noRecord;
        public GameObject onRecord;

        public InputField nameInputField;

        public Text nameContentText;
        public Text chapterContentText;
        public Text sumTimeContentText;
        public TimeUnitShow timeUnitShow;
        public Text lastTimeContentText;

        private SaveContent saveContent;

        // 根据存档是否存在，激活相应的显示界面
        private void Start()
        {
            if (SaveUtility.Exist(saveID))
            {
                onRecord.SetActive(true);
                saveContent = SaveUtility.Load(saveID);

                // 显示存档信息
                nameContentText.text = saveContent.gameData.name;
                chapterContentText.text = saveContent.gameData.chapter.ToString();
                sumTimeContentText.text = SumTimeToString();
                lastTimeContentText.text = saveContent.gameData.lastTime;
            }
            else
            {
                noRecord.SetActive(true);
            }
        }

        // 将存档里的总时间转化成合适的字符串输出
        private string SumTimeToString()
        {
            if (saveContent.gameData.sumTime < 60f)
            {
                timeUnitShow.Second();
                return saveContent.gameData.sumTime.ToString("f2");
            }
            else if (saveContent.gameData.sumTime < 3600f)
            {
                timeUnitShow.Minute();
                return (saveContent.gameData.sumTime / 60f).ToString("f2");
            }
            else
            {
                timeUnitShow.Hour();
                return (saveContent.gameData.sumTime / 3600f).ToString("f2");
            }
        }

        /// <summary>
        /// 新建一个初始存档，开始新游戏
        /// </summary>
        public void StartNewGame()
        {
            saveContent = new SaveContent(nameInputField.text);
            SaveUtility.currentSaveID = saveID;
            SaveUtility.Save(saveID, saveContent);
            SceneManager.LoadScene("Chapter " + saveContent.gameData.chapter);
        }

        /// <summary>
        /// 设置好当前存档，跳转到相应的场景
        /// </summary>
        public void Load()
        {
            SaveUtility.currentSaveID = saveID;
            SceneManager.LoadScene("Chapter " + saveContent.gameData.chapter);
        }

        /// <summary>
        /// 删除本存档，并跳转到无存档界面
        /// </summary>
        public void Delete()
        {
            SaveUtility.Delete(saveID);

            onRecord.SetActive(false);
            noRecord.SetActive(true);
        }
    }
}