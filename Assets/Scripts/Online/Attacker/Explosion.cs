using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Explosion : NetworkBehaviour, IExplosion
    {
        [SyncVar] int m_Attack; // 攻击力由生产者决定
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        [SyncVar] GameObject m_Owner; // 拥有者在被创建时被赋值
        public GameObject owner { get { return m_Owner; } set { m_Owner = value; } }

        private CommonExplosion<Explosion> commonExplosion; // 公共实现

        private void Awake()
        {
            commonExplosion = new CommonExplosion<Explosion>(this);
        }

        // 对碰到爆炸的单位造成伤害
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D collider)
        {
            commonExplosion.OnTriggerEnter2D(collider);
        }
    }
}