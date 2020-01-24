using UnityEngine;

/// <summary>
/// 弹药攻击者，包括子弹，导弹，激光等。
/// </summary>
public interface IAmmoAttacker : IAttacker
{
    void OnAttack(IState state, Collision2D collision);
    void OnDeath(Collision2D collision);
}
