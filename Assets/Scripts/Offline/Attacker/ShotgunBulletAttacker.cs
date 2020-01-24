using UnityEngine;

namespace Offline
{
    public class ShotgunBulletAttacker : AmmoAttacker, IShotgunBulletAttacker
    {
        [SerializeField] int m_ExtraNumBullets = 10; // 额外子弹个数
        public int extraNumBullets { get { return m_ExtraNumBullets; } set { m_ExtraNumBullets = value; } }

        bool m_IsHost = true; // 宿主子弹
        public bool isHost { get { return m_IsHost; } set { m_IsHost = value; } }

        int m_ShotgunBulletId; // 本发散弹的id
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
            commonShotgunBulletAttacker.Start();
        }

        // 直接攻击
        public override void OnAttack(IState state, Collision2D collision) { commonAmmoAttacker.OnDirectAttack(state, collision); }

        // 不会与同一次发出的其它散弹互相伤害
        public override void OnDeath(Collision2D collision)
        {
            if (commonShotgunBulletAttacker.ShouldDeath(collision))
            {
                Destroy(gameObject);
            }
        }
    }
}