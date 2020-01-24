using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单位状态接口。
/// </summary>
public interface IState
{
    int maxHealth { get; set; }
    int health { get; set; }
    int defense { get; set; }
    bool canBeStunned { get; set; }
    float stun { get; set; }
    Group group { get; set; }
    SpriteRenderer spriteRenderer { get; set; }
    RawImage healthBar { get; set; }
    GameObject damagedEffectPrefab { get; set; }
    int layer { get; set; }

    void TakeAttack(int attack);
    void DamagedEffect(Vector2 effectPosition);
    void Resurrection();
}