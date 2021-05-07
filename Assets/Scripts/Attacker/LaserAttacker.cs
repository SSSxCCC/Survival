using UnityEngine;
using UnityEngine.Networking;

public class LaserAttacker : AmmoAttacker {
    // 直接攻击
    [Server]
    public override void OnAttack(State state, Collision2D collision) {
        OnDirectAttack(state, collision);
    }

    // 并不会直接销毁
    [Server]
    public override void OnDeath(Collision2D collision) { }
}
