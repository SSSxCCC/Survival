using UnityEngine;

namespace Offline
{
    public class LaserAttacker : AmmoAttacker
    {
        // 直接攻击
        public override void OnAttack(IState state, Collision2D collision) { commonAmmoAttacker.OnDirectAttack(state, collision); }

        // 并不会直接销毁
        public override void OnDeath(Collision2D collision) { }
    }
}