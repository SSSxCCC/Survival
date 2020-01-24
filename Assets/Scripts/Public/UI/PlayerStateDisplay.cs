using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 此类用来显示玩家当前状态各项数值。
/// </summary>
public class PlayerStateDisplay : MonoBehaviour
{
    public static PlayerStateDisplay singleton; // 单例

    public Text healthText; // 显示血量文本框
    public Text defenseText; // 显示防御力文本框

    private void Awake()
    {
        singleton = this;
    }

    /// <summary>
    /// 设置血量情况。
    /// </summary>
    /// <param name="health">当前生命值</param>
    /// <param name="maxHealth">最大生命值</param>
    public void SetHealth(int health, int maxHealth)
    {
        healthText.text = health + "/" + maxHealth;
    }
    
    /// <summary>
    /// 设置防御力。
    /// </summary>
    /// <param name="defense">防御力</param>
    public void SetDefense(int defense)
    {
        defenseText.text = defense.ToString();
    }
}
