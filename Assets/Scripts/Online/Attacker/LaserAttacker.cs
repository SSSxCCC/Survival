using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class LaserAttacker : AmmoAttacker
    {
        // 直接攻击
        [Server]
        public override void OnAttack(IState state, Collision2D collision)
        {
            commonAmmoAttacker.OnDirectAttack(state, collision);
        }

        // 并不会直接销毁
        [Server]
        public override void OnDeath(Collision2D collision) { }
    }
}