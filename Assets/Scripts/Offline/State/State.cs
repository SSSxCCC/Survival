using UnityEngine;
using UnityEngine.UI;

namespace Offline
{
    public abstract class State : MonoBehaviour, IState
    {
        [SerializeField] int m_MaxHealth = 1; // 最大生命值
        public int maxHealth { get { return m_MaxHealth; }
            set {
                m_MaxHealth = value;
                if (m_MaxHealth < health) health = m_MaxHealth;
                else
                {
                    commonState.OnChangeHealth();
                    OnChangeHp();
                }
            }
        }

        [SerializeField] int m_Health = 1; // 当前生命值
        public int health { get { return m_Health; }
            set {
                m_Health = Mathf.Min(value, maxHealth); // 阻止当前生命值超过最大生命值
                commonState.OnChangeHealth();
                if (m_Health <= 0) OnEmptyHealth();
                OnChangeHp();
            }
        }

        [SerializeField] int m_Defense; // 防御力
        public int defense { get { return m_Defense; } set { m_Defense = value; OnChangeDfs(); } }

        [SerializeField] bool m_CanBeStunned; // 可被击晕的
        public bool canBeStunned { get { return m_CanBeStunned; } set { m_CanBeStunned = value; } }

        float m_Stun; // 眩晕时间
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

        public int layer { get { return gameObject.layer; } set { commonState.OnChangeLayer(value); } } // 单位所在物理层

        private CommonState<State> commonState; // 公共实现

        protected virtual void Awake()
        {
            commonState = new CommonState<State>(this);
            commonState.Awake(); // 开始时满血
            commonState.OnChangeLayer(gameObject.layer); // 正常显示物理层情况
        }

        // 更新眩晕时间
        private void Update() { commonState.Update(); }

        // 受到攻击力为attack的攻击
        public void TakeAttack(int attack) { commonState.TakeAttack(attack); }

        // 空血时调用
        protected abstract void OnEmptyHealth();

        // 受伤显示特效
        public void DamagedEffect(Vector2 effectPosition) { commonState.DamagedEffect(effectPosition); }

        // 复活
        public void Resurrection() { commonState.Resurrection(); }

        // 生命值或最大生命值改变时调用，此方法为了子类而存在
        protected virtual void OnChangeHp() { }

        // 防御力改变时调用，此方法为了子类而存在
        protected virtual void OnChangeDfs() { }
    }
}