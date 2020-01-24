using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class ObjectSaverManager : MonoBehaviour
    {
        public static ObjectSaverManager singleton; // 单例
        
        private HashSet<ObjectSaver> objectSaverSet = new HashSet<ObjectSaver>(); // 每个需要存档的物体的存档脚本

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else if (singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject); // 防止因载入场景而被销毁
        }

        /// <summary>
        /// 得到每个物体存档数据。
        /// </summary>
        /// <returns>每个物体存档数据</returns>
        public List<ObjectSaver.Data> GetObjectDataList()
        {
            List<ObjectSaver> tempObjectSaverList = new List<ObjectSaver>();
            int i = 0;
            foreach (ObjectSaver objectSaver in objectSaverSet)
            {
                objectSaver.tempIndex = i;
                tempObjectSaverList.Add(objectSaver);
                i++;
            }

            List<ObjectSaver.Data> objectDataList = new List<ObjectSaver.Data>();
            foreach (ObjectSaver objectSaver in tempObjectSaverList)
            {
                objectDataList.Add(objectSaver.SaveToData());
            }
            return objectDataList;
        }

        /// <summary>
        /// 存档物体脚本注册。
        /// </summary>
        /// <param name="objectSaver">注册的存档脚本</param>
        public void Register(ObjectSaver objectSaver)
        {
            objectSaverSet.Add(objectSaver);
        }

        /// <summary>
        /// 存档物体脚本取消注册。
        /// </summary>
        /// <param name="objectSaver">取消注册的存档脚本</param>
        public void Unregister(ObjectSaver objectSaver)
        {
            objectSaverSet.Remove(objectSaver);
        }
    }
}