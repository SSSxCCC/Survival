using UnityEngine;

namespace Offline
{
    public abstract class AmmoAttacker : MonoBehaviour, IAmmoAttacker
    {
        int m_Attack; // 攻击力由发射器决定
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        GameObject m_Owner; // 拥有者在被创建时被赋值
        public GameObject owner { get { return m_Owner; } set { m_Owner = value; } }

        protected CommonAmmoAttacker<AmmoAttacker> commonAmmoAttacker; // 公共实现

        protected virtual void Awake() { commonAmmoAttacker = new CommonAmmoAttacker<AmmoAttacker>(this); }

        // 弹药初始化
        protected virtual void Start() { commonAmmoAttacker.Start(); }

        // 子弹撞击判定造成伤害
        private void OnCollisionEnter2D(Collision2D collision) { commonAmmoAttacker.OnCollisionEnter2D(collision); }

        // 判定攻击
        public abstract void OnAttack(IState state, Collision2D collision);

        // 判定毁掉自己
        public abstract void OnDeath(Collision2D collision);
    }
}