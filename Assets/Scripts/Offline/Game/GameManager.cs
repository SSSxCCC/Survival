using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Offline
{
    public abstract class GameManager : MonoBehaviour
    {
        public static GameManager singleton;

        public GameObject playerPrefab; // 玩家单位
        public Transform startPlace; // 玩家初始位置

        /// <summary>
        /// 单位死亡事件，在有玩家或敌人死亡时调用
        /// </summary>
        public event DeathEventHandler Death;
        public delegate void DeathEventHandler(GameObject deadObject);

        public Text hintUIText; // 游戏界面提示文本

        public LocalizationItem completeItem;
        public LocalizationItem terminatedItem;

        [HideInInspector] public bool initialization; // 游戏是否初始化（从本章起始开始），由Awake里面读取的存档来决定。

        protected GameObject player; // 玩家对象

        private List<Mission> missionList; // 任务列表

        private Queue<HintText> textQueue; // 游戏提示文本队列
        private bool showing = false; // 当前有文本正在显示

        private SaveContent saveContent; // 游戏存档内容
        private float startTime; // 本章开始时间

        // 游戏初始化
        private void Awake()
        {
            // 成为单例
            singleton = this;

            // 变量初始化
            textQueue = new Queue<HintText>();
            missionList = new List<Mission>();
            startTime = Time.time; // 记录本章开始时间

            // 读取存档内容
            saveContent = SaveUtility.Load(SaveUtility.currentSaveID);
            initialization = (saveContent.objectDataList == null || saveContent.objectDataList.Count == 0);

            if (initialization) // 如果从本章初始开始
            {   // 仅需要刷出玩家
                player = Instantiate(playerPrefab, startPlace.position, Quaternion.identity);
                LoadPlayerState();
            }
            else
                LoadGame();
        }

        /// <summary>
        /// 从存档读取每个物体信息以及每个任务信息并将其还原。
        /// </summary>
        private void LoadGame()
        {
            // 每个物体
            List<GameObject> loadedObjectList = new List<GameObject>();
            foreach (ObjectSaver.Data data in saveContent.objectDataList)
            {
                GameObject prefab = SinglePlayerManager.singleton.savablePrefabDictionary[data.prefabKey];
                GameObject loadedObject = Instantiate(prefab, data.position, Quaternion.Euler(0, 0, data.degree));
                loadedObject.GetComponent<ObjectSaver>().LoadStringData(data.stringData);
                if (prefab == playerPrefab)
                {
                    player = loadedObject;
                    LoadPlayerState();
                }
                loadedObjectList.Add(loadedObject);
            }
            foreach (GameObject loadedObject in loadedObjectList)
            {
                loadedObject.GetComponent<ObjectSaver>().LoadMoreData(loadedObjectList);
            }

            // 每个任务
            if (saveContent.missionDataList == null || saveContent.missionDataList.Count <= 0)
                Invoke("FinishChapter", 1f);
            else
            {
                foreach (Mission.Data data in saveContent.missionDataList)
                {
                    Mission mission = SinglePlayerManager.singleton.savableMissionDictionary[data.missionKey];
                    mission.LoadFromData(data.missionData);
                }
            }
        }

        // 将玩家状态调整到与存档内容保持一致
        private void LoadPlayerState()
        {
            IState state = player.GetComponent<IState>();
            state.maxHealth = saveContent.playerData.maxHealth;
            state.health = saveContent.playerData.health;
            state.defense = saveContent.playerData.defense;

            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            if (saveContent.playerData.pistolBullet >= 0)
            {
                GameObject pistol = Instantiate(SinglePlayerManager.singleton.pistolPrefab);
                pistol.GetComponent<Weapon>().numAmmo = saveContent.playerData.pistolBullet;
                weaponManager.PickUpWeapon(pistol);
            }
            if (saveContent.playerData.shotgunBullet >= 0)
            {
                GameObject shotgun = Instantiate(SinglePlayerManager.singleton.shotgunPrefab);
                shotgun.GetComponent<Weapon>().numAmmo = saveContent.playerData.shotgunBullet;
                weaponManager.PickUpWeapon(shotgun);
            }
            if (saveContent.playerData.machineGunBullet >= 0)
            {
                GameObject machineGun = Instantiate(SinglePlayerManager.singleton.machineGunPrefab);
                machineGun.GetComponent<Weapon>().numAmmo = saveContent.playerData.machineGunBullet;
                weaponManager.PickUpWeapon(machineGun);
            }
            if (saveContent.playerData.rocket >= 0)
            {
                GameObject rocketLauncher = Instantiate(SinglePlayerManager.singleton.rocketLauncherPrefab);
                rocketLauncher.GetComponent<Weapon>().numAmmo = saveContent.playerData.rocket;
                weaponManager.PickUpWeapon(rocketLauncher);
            }
            if (saveContent.playerData.laser >= 0)
            {
                GameObject laserGun = Instantiate(SinglePlayerManager.singleton.laserGunPrefab);
                laserGun.GetComponent<Weapon>().numAmmo = saveContent.playerData.laser;
                weaponManager.PickUpWeapon(laserGun);
            }
        }

        // 敌人死亡时调用
        public virtual void OnEnemyDeath(GameObject enemy)
        {
            if (Death != null) Death(enemy);
        }
        
        /// <summary>
        /// 任务完成时被调用，在任务列表删除此任务，
        /// 开始下一个任务或者调用<see cref="FinishChapter"/>结束本章。
        /// </summary>
        /// <param name="mission">完成的任务</param>
        public virtual void OnMissionComplete(Mission mission)
        {
            missionList.Remove(mission);
        }

        // 玩家死亡时调用
        public virtual void OnPlayerDeath()
        {
            if (Death != null) Death(player);
            ShowText(new HintText(terminatedItem.GetText(), Color.red, 3f));
        }

        /// <summary>
        /// 本章游戏结束时调用，保存游戏，然后进入下一章
        /// </summary>
        protected void FinishChapter()
        {
            saveContent.gameData.chapter++;
            saveContent.gameData.sumTime += (Time.time - startTime);
            saveContent.gameData.lastTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
            SavePlayerState();
            saveContent.objectDataList = null;
            
            SaveUtility.Save(SaveUtility.currentSaveID, saveContent);

            SceneManager.LoadScene("Chapter " + saveContent.gameData.chapter);
        }
        
        // 保存玩家状态
        private void SavePlayerState()
        {
            IState state = player.GetComponent<IState>();
            saveContent.playerData.maxHealth = state.maxHealth;
            saveContent.playerData.health = state.health;
            saveContent.playerData.defense = state.defense;

            WeaponManager weaponManager = player.GetComponent<WeaponManager>();
            GameObject weapon;
            if ((weapon = weaponManager.GetWeapon(WeaponType.Pistol)) == null) saveContent.playerData.pistolBullet = -1;
            else saveContent.playerData.pistolBullet = weapon.GetComponent<Weapon>().numAmmo;
            if ((weapon = weaponManager.GetWeapon(WeaponType.Shotgun)) == null) saveContent.playerData.shotgunBullet = -1;
            else saveContent.playerData.shotgunBullet = weapon.GetComponent<Weapon>().numAmmo;
            if ((weapon = weaponManager.GetWeapon(WeaponType.MachineGun)) == null) saveContent.playerData.machineGunBullet = -1;
            else saveContent.playerData.machineGunBullet = weapon.GetComponent<Weapon>().numAmmo;
            if ((weapon = weaponManager.GetWeapon(WeaponType.RocketLauncher)) == null) saveContent.playerData.rocket = -1;
            else saveContent.playerData.rocket = weapon.GetComponent<Weapon>().numAmmo;
            if ((weapon = weaponManager.GetWeapon(WeaponType.LaserGun)) == null) saveContent.playerData.laser = -1;
            else saveContent.playerData.laser = weapon.GetComponent<Weapon>().numAmmo;
        }

        /// <summary>
        /// 即时保存游戏。
        /// </summary>
        public void SaveGame()
        {
            saveContent.gameData.sumTime += (Time.time - startTime);
            startTime = Time.time; // 重新记时
            saveContent.gameData.lastTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            SavePlayerState();

            saveContent.objectDataList = ObjectSaverManager.singleton.GetObjectDataList();

            saveContent.missionDataList = new List<Mission.Data>();
            foreach (Mission mission in missionList) saveContent.missionDataList.Add(mission.SaveToData());

            SaveUtility.Save(SaveUtility.currentSaveID, saveContent);
        }

        /// <summary>
        /// 注册任务，在任务的Start方法里面调用
        /// </summary>
        /// <param name="mission">任务</param>
        public void RegisterMission(Mission mission)
        {
            missionList.Add(mission);
        }

        /// <summary>
        /// 显示所有任务提示
        /// </summary>
        public void ShowMissionDescriptions()
        {
            foreach (Mission mission in missionList)
            {
                string text = mission.missionName + "  " + mission.missionDescription;
                ShowText(new HintText(text, Color.blue, 3f));
            }
        }

        /// <summary>
        /// 在游戏UI界面显示一个提示文本。
        /// </summary>
        /// <param name="hintText">提示文本内容</param>
        public void ShowText(HintText hintText)
        {
            if (showing)
            {
                textQueue.Enqueue(hintText);
            }
            else
            {
                showing = true;
                hintUIText.text = hintText.text;
                hintUIText.color = hintText.color;
                Invoke("TextTimeUp", hintText.duration);
            }
        }

        // 当前显示的文本时间到了
        private void TextTimeUp()
        {
            if (textQueue.Count > 0)
            {
                HintText nextHintText = textQueue.Dequeue();
                hintUIText.text = nextHintText.text;
                hintUIText.color = nextHintText.color;
                Invoke("TextTimeUp", nextHintText.duration);
            }
            else
            {
                hintUIText.text = null;
                showing = false;
            }
        }
    }

    /// <summary>
    /// 游戏提示文本结构
    /// </summary>
    public struct HintText
    {
        public string text;
        public Color color;
        public float duration;

        public HintText(string text, Color color, float duration)
        {
            this.text = text;
            this.color = color;
            this.duration = duration;
        }
    }
}