using UnityEngine;

namespace Offline
{
    public class BulletAttacker : AmmoAttacker
    {
        // 直接攻击
        public override void OnAttack(IState state, Collision2D collision) { commonAmmoAttacker.OnDirectAttack(state, collision); }

        // 销毁自己
        public override void OnDeath(Collision2D collision) { Destroy(gameObject); }
    }
}