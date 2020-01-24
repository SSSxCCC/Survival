using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Online
{
    public abstract class State : NetworkBehaviour, IState
    {
        [SerializeField] [SyncVar(hook = "OnChangeMaxHealth")] int m_MaxHealth = 1; // 最大生命值
        public int maxHealth { get { return m_MaxHealth; }
            set {
                m_MaxHealth = value;
                if (m_MaxHealth < health) health = m_MaxHealth;
            }
        }

        [SerializeField] [SyncVar(hook = "OnChangeHealth")] int m_Health = 1; // 当前生命值
        public int health { get { return m_Health; }
            set {
                m_Health = Mathf.Min(value, maxHealth); // 阻止当前生命值超过最大生命值
                if (isServer && m_Health <= 0) OnEmptyHealth();
            }
        }

        [SerializeField] [SyncVar(hook = "OnChangeDefense")] int m_Defense; // 防御力
        public int defense { get { return m_Defense; } set { m_Defense = value; } }

        [SerializeField] bool m_CanBeStunned; // 可被击晕的
        public bool canBeStunned { get { return m_CanBeStunned; } set { m_CanBeStunned = value; } }

        [SyncVar] float m_Stun; // 眩晕时间
        public float stun { get { return m_Stun; }
            set {
                if (canBeStunned)
                    m_Stun = value > 0 ? value : 0;
            }
        }

        [SerializeField] Group m_Group; // 所属的组
        public Group group { get { return m_Group; } set { m_Group = value; } }

        [SerializeField] SpriteRenderer m_SpriteRenderer; // 渲染器，用于死亡染色
        public SpriteRenderer spriteRenderer { get { return m_SpriteRenderer; } set { m_SpriteRenderer = value; } }

        [SerializeField] RawImage m_HealthBar; // 血管
        public RawImage healthBar { get { return m_HealthBar; } set { m_HealthBar = value; } }

        [SerializeField] GameObject m_DamagedEffectPrefab; // 受伤特效
        public GameObject damagedEffectPrefab { get { return m_DamagedEffectPrefab; } set { m_DamagedEffectPrefab = value; } }

        [SyncVar(hook = "OnChangeLayer")] int m_Layer = -1; // 单位所在物理层
        public int layer { get { return m_Layer >= 0 ? m_Layer : gameObject.layer; } set { m_Layer = value; } }

        private CommonState<State> commonState; // 公共实现

        protected virtual void Awake()
        {
            commonState = new CommonState<State>(this);
            
            commonState.Awake(); // 当前生命值等于最大生命值
        }

        // 更新眩晕时间
        [ServerCallback]
        private void Update()
        {
            commonState.Update();
        }

        // 受到攻击力为attack的攻击
        [Server]
        public void TakeAttack(int attack)
        {
            commonState.TakeAttack(attack);
        }

        // 空血时调用
        [Server]
        protected abstract void OnEmptyHealth();

        // 生命值改变时客户端调整相应血管显示
        [Client]
        private void OnChangeHealth(int health)
        {
            m_Health = health;
            commonState.OnChangeHealth();
            OnChangeHp();
        }

        // 最大生命值改变时客户端调整相应血管显示
        [Client]
        private void OnChangeMaxHealth(int maxHealth)
        {
            m_MaxHealth = maxHealth;
            if (maxHealth >= health) commonState.OnChangeHealth();
            OnChangeHp();
        }

        // 防御力改变时调用
        [Client]
        private void OnChangeDefense(int defense)
        {
            m_Defense = defense;
            OnChangeDfs();
        }

        // 生命值或最大生命值改变时调用，此方法为了子类而存在
        [Client]
        protected virtual void OnChangeHp() { }

        // 防御力改变时调用，此方法为了子类而存在
        [Client]
        protected virtual void OnChangeDfs() { }

        // 受伤显示特效
        [Server]
        public void DamagedEffect(Vector2 effectPosition)
        {
            GameObject damagedEffect = commonState.DamagedEffect(effectPosition);
            if (damagedEffect != null) NetworkServer.Spawn(damagedEffect);
        }

        // 复活
        [Server]
        public void Resurrection()
        {
            commonState.Resurrection();
        }

        // 单位所属层改变时调用
        [Client]
        private void OnChangeLayer(int layer)
        {
            m_Layer = layer;
            commonState.OnChangeLayer(layer);
        }

        // 为新来的玩家显示之前改过的东西
        [Client]
        public override void OnStartClient()
        {
            commonState.OnChangeHealth(); // 正常显示之前死亡单位和单位血条
            commonState.OnChangeLayer(layer); // 正常显示对layer的更改
        }
    }
}