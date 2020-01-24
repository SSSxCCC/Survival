using System;
using System.Collections.Generic;

namespace Offline
{
    /// <summary>
    /// 一个游戏存档的所有内容，包括游戏记录，世界中每个单位信息等内容。
    /// </summary>
    [Serializable]
    public class SaveContent
    {
        public GameData gameData = new GameData(); // 游戏数据

        public PlayerData playerData = new PlayerData(); // 玩家数据

        public List<ObjectSaver.Data> objectDataList; // 每个物体存档数据

        public List<Mission.Data> missionDataList; // 每个任务存档数据

        // 游戏数据
        [Serializable]
        public class GameData
        {
            // 游戏记录
            public string name;
            public int chapter;
            public float sumTime;
            public string lastTime;
        }

        // 玩家数据
        [Serializable]
        public class PlayerData
        {
            // 状态属性
            public int maxHealth;
            public int health;
            public int defense;

            // 武器情况
            public int pistolBullet;
            public int shotgunBullet;
            public int machineGunBullet;
            public int rocket;
            public int laser;
        }

        /// <summary>
        /// 创建一个新游戏的初始存档，其内容自动初始化
        /// </summary>
        /// <param name="name">玩家给此存档取的名字</param>
        public SaveContent(string name)
        {
            gameData.name = name;
            gameData.chapter = 1;
            gameData.sumTime = 0;
            gameData.lastTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            playerData.maxHealth = 100;
            playerData.health = 100;
            playerData.defense = 0;

            playerData.pistolBullet = int.MaxValue; // 手枪子弹无限
            playerData.shotgunBullet = -1;
            playerData.machineGunBullet = -1;
            playerData.rocket = -1;
            playerData.laser = -1;

            objectDataList = null;
            missionDataList = null;
        }
    }
}