using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class ShotgunBulletAttacker : AmmoAttacker, IShotgunBulletAttacker
    {
        [SerializeField] int m_ExtraNumBullets = 10; // 额外子弹个数
        public int extraNumBullets { get { return m_ExtraNumBullets; } set { m_ExtraNumBullets = value; } }

        bool m_IsHost = true; // 宿主子弹
        public bool isHost { get { return m_IsHost; } set { m_IsHost = value; } }

        [SyncVar] int m_ShotgunBulletId; // 本发散弹的id（如果没有SyncVar，主机切换时可能会有点问题）
        public int shotgunBulletId { get { return m_ShotgunBulletId; } set { m_ShotgunBulletId = value; } }

        private CommonShotgunBulletAttacker<ShotgunBulletAttacker> commonShotgunBulletAttacker; // 公共实现类

        protected override void Awake()
        {
            base.Awake();
            commonShotgunBulletAttacker = new CommonShotgunBulletAttacker<ShotgunBulletAttacker>(this);
        }

        // 宿主子弹复制自己生成所有额外子弹
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;

            GameObject[] bullets = commonShotgunBulletAttacker.Start();

            if (bullets != null)
            {
                foreach (GameObject bullet in bullets) NetworkServer.Spawn(bullet);
            }
        }

        // 直接攻击
        [Server]
        public override void OnAttack(IState state, Collision2D collision)
        {
            commonAmmoAttacker.OnDirectAttack(state, collision);
        }

        // 不会与同一次发出的其它散弹互相伤害
        [Server]
        public override void OnDeath(Collision2D collision)
        {
            if (commonShotgunBulletAttacker.ShouldDeath(collision))
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}