using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class RocketAttacker : AmmoAttacker, IRocketAttacker
    {
        [SerializeField] float m_Propulsion; // 推进力
        public float propulsion { get { return m_Propulsion; } set { m_Propulsion = value; } }

        [SerializeField] GameObject m_ExplosionPrefab; // 爆炸的prefab
        public GameObject explosionPrefab { get { return m_ExplosionPrefab; } set { m_ExplosionPrefab = value; } }

        private CommonRocketAttacker<RocketAttacker> commonRocketAttacker; // 公共实现

        protected override void Awake()
        {
            base.Awake();
            commonRocketAttacker = new CommonRocketAttacker<RocketAttacker>(this);
        }

        protected override void Start()
        {
            base.Start();
            commonRocketAttacker.Start();
        }

        // 施加推进力
        [ServerCallback]
        private void FixedUpdate()
        {
            commonRocketAttacker.FixedUpdate();
        }

        // 不需要直接造成伤害
        [Server]
        public override void OnAttack(IState state, Collision2D collision) { }

        // 产生爆炸造成伤害
        [Server]
        public override void OnDeath(Collision2D collision)
        {
            if (Random.value < 0.1f) // 劣质的导弹
                return;

            // 创造爆炸
            NetworkServer.Spawn(commonRocketAttacker.CreateExplosion());

            NetworkServer.Destroy(gameObject);
        }
    }
}