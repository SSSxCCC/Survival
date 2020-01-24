using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Offline
{
    public class SaveUtility
    {
        private static int m_currentSaveID = 1;
        /// <summary>
        /// 目前单机模式对应的第几个存档。
        /// 在单机模式开始前设置好，不要在单机模式中途更改。
        /// </summary>
        public static int currentSaveID { get { return m_currentSaveID; }
            set { // 保证currentArchiveID的值有效
                if (value >= 1 && value <= 3)
                    m_currentSaveID = value;
                else
                    Debug.LogWarning("You tried to set currentSaveID to an invalid value: " + value);
            }
        }

        /// <summary>
        /// 存档路径
        /// </summary>
        public static string saveFolderPath { get { return Path.Combine(Application.persistentDataPath, "Saves"); } }
        
        private static string GetSaveFilePath(int saveID)
        {
            return Path.Combine(saveFolderPath, saveID + ".save");
        }

        /// <summary>
        /// 判断存档ID对应的存档是否存在于硬盘。
        /// </summary>
        /// <param name="saveID">被判断的存档ID</param>
        /// <returns>若存档ID对应的存档存在，则返回true，否则返回false。</returns>
        public static bool Exist(int saveID)
        {
            return File.Exists(GetSaveFilePath(saveID));
        }

        /// <summary>
        /// 从硬盘载入存档ID对应的游戏存档，得到存档内容。
        /// </summary>
        /// <param name="saveID">载入的存档ID</param>
        /// <returns>返回存档内容，如果存档不存在，则返回null</returns>
        public static SaveContent Load(int saveID)
        {
            if (!Exist(saveID)) return null;

            string fromJson = File.ReadAllText(GetSaveFilePath(saveID));
            SaveContent saveContent = JsonUtility.FromJson<SaveContent>(fromJson);
            //Stream loadStream = File.Open(GetSaveFilePath(saveID), FileMode.Open, FileAccess.Read);
            //SaveContent saveContent = (SaveContent)new BinaryFormatter().Deserialize(loadStream);
            //loadStream.Close();

            return saveContent;
        }

        /// <summary>
        /// 将存档保存到硬盘上，覆盖存档ID对应的游戏存档。
        /// </summary>
        /// <param name="saveID">保存的存档ID</param>
        /// <param name="saveContent">保存的存档内容</param>
        public static void Save(int saveID, SaveContent saveContent)
        {
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);

            string toJson = JsonUtility.ToJson(saveContent);
            File.WriteAllText(GetSaveFilePath(saveID), toJson);
            //Stream saveStream = File.Open(GetSaveFilePath(saveID), FileMode.OpenOrCreate, FileAccess.Write);
            //new BinaryFormatter().Serialize(saveStream, saveContent);
            //saveStream.Close();
        }

        /// <summary>
        /// 删除存档ID对应的存档。
        /// </summary>
        /// <param name="saveID">被删除的存档ID</param>
        public static void Delete(int saveID)
        {
            if (!Exist(saveID)) return;

            File.Delete(GetSaveFilePath(saveID));
        }
    }
}