using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public abstract class ObjectSaver : MonoBehaviour
    {
        public string prefabKey;

        /// <summary>
        /// 在遍历保存每个物体的循环前（<see cref="SaveToData"/>方法被调用前）获得唯一值，方便存储物体间相互关联的信息。
        /// </summary>
        [HideInInspector] public int tempIndex;

        private void Start()
        {
            ObjectSaverManager.singleton.Register(this);
        }

        private void OnDestroy()
        {
            ObjectSaverManager.singleton.Unregister(this);
        }

        public Data SaveToData()
        {
            return new Data(prefabKey, transform.position, transform.eulerAngles.z, GetStringData());
        }

        protected abstract string GetStringData();

        public abstract void LoadStringData(string stringData);

        public abstract void LoadMoreData(List<GameObject> loadedObjectList);

        /// <summary>
        /// 单个物体存档内容。
        /// </summary>
        [Serializable]
        public class Data
        {
            public string prefabKey;
            public Vector2 position;
            public float degree;
            public string stringData;

            public Data(string prefabKey, Vector2 position, float degree, string stringData)
            {
                this.prefabKey = prefabKey;
                this.position = position;
                this.degree = degree;
                this.stringData = stringData;
            }
        }
    }
}