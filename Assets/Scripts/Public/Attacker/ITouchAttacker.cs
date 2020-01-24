using UnityEngine;

/// <summary>
/// 触碰攻击者接口。
/// </summary>
public interface ITouchAttacker : IAttacker
{
    float assaultInterval { get; set; }
    float thrust { get; set; }
    float lastAssaultTime { get; set; }

    void Push(GameObject pushedObject, Vector2 contactPoint);
}
