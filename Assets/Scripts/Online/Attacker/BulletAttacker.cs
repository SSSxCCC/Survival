using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class BulletAttacker : AmmoAttacker
    {
        // 直接攻击
        [Server]
        public override void OnAttack(IState state, Collision2D collision)
        {
            commonAmmoAttacker.OnDirectAttack(state, collision);
        }

        // 销毁自己
        [Server]
        public override void OnDeath(Collision2D collision)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}