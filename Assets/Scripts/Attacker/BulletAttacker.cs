using UnityEngine;
using UnityEngine.Networking;

public class BulletAttacker : AmmoAttacker {

    // 直接攻击
    [Server]
    public override void OnAttack(State state, Collision2D collision) {
        OnDirectAttack(state, collision);
    }

    // 销毁自己
    [Server]
    public override void OnDeath(Collision2D collision) {
        NetworkServer.Destroy(gameObject);
    }
}
