using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class SinglePlayerManager : MonoBehaviour
    {
        public static SinglePlayerManager singleton;

        // 单机模式武器
        public GameObject pistolPrefab;
        public GameObject shotgunPrefab;
        public GameObject machineGunPrefab;
        public GameObject rocketLauncherPrefab;
        public GameObject laserGunPrefab;

        // 单机模式prefab
        public List<GameObject> savablePrefabList;
        public Dictionary<string, GameObject> savablePrefabDictionary = new Dictionary<string, GameObject>();

        // 单机模式任务
        public List<Mission> savableMissionList;
        public Dictionary<string, Mission> savableMissionDictionary = new Dictionary<string, Mission>();

        // 单例模式
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

            Initialize();
        }

        private void Initialize()
        {
            foreach (GameObject prefab in savablePrefabList)
            {
                savablePrefabDictionary[prefab.GetComponent<ObjectSaver>().prefabKey] = prefab;
            }

            foreach (Mission mission in savableMissionList)
            {
                savableMissionDictionary[mission.missionKey] = mission;
            }
        }
    }
}