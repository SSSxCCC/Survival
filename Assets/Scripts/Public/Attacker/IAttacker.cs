using UnityEngine;

/// <summary>
/// 攻击者接口。
/// </summary>
public interface IAttacker
{
    int attack { get; set; }
    GameObject owner { get; set; }
}
