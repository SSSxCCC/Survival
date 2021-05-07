using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour {
    public static GameManager singleton; // 单例

    public MapBundle mapBundle; // 地图集
    public Text levelText; // 在新的一关中显示游戏等级文本
    
    public EnemyTactics[] firstFewLevels; // 前几关刷敌策略
    public EnemyTactics[] seniorLevels; // 后面关卡刷敌策略
    public GameObject[] vehiclePrefabs; // 所有可以刷的载具
    public GameObject[] awardDroppedWeaponPrefabs; // 奖励掉落武器

    public LocalizationItem levelItem;
    public LocalizationItem gameOverInLevelItem;

    private static List<Transform> enemyStartPositions = new List<Transform>(); // 所有可以刷敌人的位置
    public static List<Transform> vehicleStartPositions = new List<Transform>(); // 所有可以刷载具的位置（带朝向的）

    // 以下几个同步变量是为当原来的主机退出后产生的新主机准备的
    [HideInInspector] [SyncVar] public bool hurtEachOther; // 玩家是否可以互相伤害
    [SyncVar] private string map; // 游戏地图名
    [SyncVar] private int level = 1; // 游戏等级（第几关）
    [SyncVar] private bool spawning = true; // 指示是否还有敌方单位要刷
    [SyncVar] private int numSpawnedEnemy = 0; // 已刷敌人数量
    [SyncVar] private int tacticsIndex; // 刷敌策略下标
    
    // 自己成为单例，并关掉广播或监听
    private void Awake() {
        singleton = this;

        if (MyNetworkDiscovery.singleton.running)
            MyNetworkDiscovery.singleton.StopBroadcast();
    }

    // 新的主机到来时，按照游戏规则继续，开始广播
    [Server]
    public override void OnStartServer() {
        StartCoroutine("SpawnEnemies");

        Invoke("OnKilledPlayer", 1f); // 如果之前的主机是唯一幸存者，在此情况下此句话可以保证正常判定游戏结束

        MyNetworkDiscovery.singleton.Initialize();
        MyNetworkDiscovery.singleton.StartAsServer();
    }

    // 把游戏地图刷出来
    [Client]
    public override void OnStartClient() {
        Instantiate(mapBundle.nameMapDict[map != null ? map : OnlineGameSetting.map]);
    }

    // 初始化游戏
    [ServerCallback]
    private void Start() {
        hurtEachOther = OnlineGameSetting.hurtEachOther;
        map = OnlineGameSetting.map;

        level = 1;
        StartGameByLevel();
    }

    // 根据level初始化游戏
    [Server]
    private void StartGameByLevel() {
        // 开始不断刷敌人
        numSpawnedEnemy = 0;
        if (level > firstFewLevels.Length) tacticsIndex = Random.Range(0, seniorLevels.Length);
        if (level > 1) StartCoroutine("SpawnEnemies");

        // 复活上一关中所有死亡的玩家
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            var state = player.GetComponent<State>();
            if (state.health <= 0) {
                state.Resurrection();
            }
        }

        // 刷新载具
        if (vehicleStartPositions.Count > 0 && GameObject.FindGameObjectsWithTag("Vehicle").Length == 0) {
            foreach (Transform vehicleSpawnTransform in vehicleStartPositions) {
                GameObject vehiclePrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
                Vehicle.Create(vehiclePrefab, vehicleSpawnTransform.position, vehicleSpawnTransform.eulerAngles.z);
            }
        }

        // 显示游戏等级（第几关）
        RpcShowLevel(level);
    }

    // 显示等级的文本
    [ClientRpc]
    private void RpcShowLevel(int level) {
        levelText.text = levelItem.GetText() + " " + level;
        Invoke("HideLevelText", 2f); // 2秒后隐藏等级文本
    }

    // 隐藏显示等级的文本
    private void HideLevelText() {
        levelText.text = null;
    }

    // 不断产生敌人
    [Server]
    private IEnumerator SpawnEnemies() {
        spawning = true;

        // 产生若干敌人
        for (; numSpawnedEnemy < level * 10; numSpawnedEnemy++) {
            yield return new WaitForSeconds(1); // 每秒刷一个敌人

            // 决定刷什么敌人
            GameObject enemyPrefab;
            if (level > firstFewLevels.Length) enemyPrefab = seniorLevels[tacticsIndex].GetNextEnemy();
            else enemyPrefab = firstFewLevels[level - 1].GetNextEnemy();

            // 刷出选择的敌人
            GameObject enemy = Instantiate(enemyPrefab, enemyStartPositions[Random.Range(0, enemyStartPositions.Count)].position, Quaternion.identity);
            if (level <= 1) enemy.GetComponent<State>().maxHealth = 1; // 降低等级一的难度
            NetworkServer.Spawn(enemy);
        }

        spawning = false;
    }

    // 有敌人死亡时被调用
    [Server]
    public void OnKilledEnemy() {
        if (spawning) {
            return;
        }

        // 统计活着的敌人总数
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        int numLiveEnemys = 0;
        foreach (GameObject enemy in allEnemies) {
            if (enemy.GetComponent<State>().health > 0) numLiveEnemys++;
        }

        // 判定是否可以进入下一关
        if (numLiveEnemys == 0) {
            if (level - 1 < awardDroppedWeaponPrefabs.Length) { // 如果是前几关
                // 过关奖励
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                    DropItem.Create(awardDroppedWeaponPrefabs[level-1], player.transform.position);
            }
            level++;
            StartGameByLevel();
        }
    }

    // 有玩家死亡时被调用
    [Server]
    public void OnKilledPlayer() {
        // 统计活着的玩家人数
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int numLivePlayers = 0;
        foreach (GameObject player in players) {
            if (player.GetComponent<State>().health > 0) numLivePlayers++;
        }

        // 判定游戏是否结束
        if (numLivePlayers == 0) {
            RpcShowEndGame(level);
        }
    }

    // 显示游戏结束并过一会儿离开联机模式
    [ClientRpc]
    private void RpcShowEndGame(int level) {
        levelText.text = gameOverInLevelItem.GetText() + " " + level;
        
        if (MyNetworkDiscovery.singleton.running)
            MyNetworkDiscovery.singleton.StopBroadcast(); // 停止广播

        Invoke("ExitOnlineGame", 3f);
    }

    // 退出联机模式，回到主菜单
    // 这里不能加[Client]，因为Invoke函数调用会认为是Server调用的
    private void ExitOnlineGame() {
        NetworkManager.singleton.StopHost(); // 关掉客户端与服务端
        
        NetworkTransport.Shutdown(); // 保证网络彻底关闭
    }

    public static void RegisterEnemyStartPosition(Transform start) {
        enemyStartPositions.Add(start);
    }

    public static void UnRegisterEnemyStartPosition(Transform start) {
        enemyStartPositions.Remove(start);
    }

    public static void RegisterVehicleStartPosition(Transform start) {
        vehicleStartPositions.Add(start);
    }

    public static void UnRegisterVehicleStartPosition(Transform start) {
        vehicleStartPositions.Remove(start);
    }
}
