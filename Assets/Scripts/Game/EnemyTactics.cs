using System;
using UnityEngine;

[Serializable]
public class EnemyTactics
{
    public Enemy[] enemies; // 定义有哪些敌人及其比例

    // 随机得到下一个敌人的类型
    public GameObject GetNextEnemy() {
        float rate = UnityEngine.Random.value;
        foreach (Enemy enemy in enemies) {
            if (rate <= enemy.ratio) {
                return enemy.enemyPrefab;
            }
        }
        return null;
    }
}

[Serializable]
public class Enemy {
    public GameObject enemyPrefab; // 敌人种类
    [Range(0, 1)] public float ratio; // 对应比例
}
