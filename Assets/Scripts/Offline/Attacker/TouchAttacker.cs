using UnityEngine;

namespace Offline
{
    public class TouchAttacker : MonoBehaviour, ITouchAttacker
    {
        [SerializeField] int m_Attack = 1; // 攻击力
        public int attack { get { return m_Attack; } set { m_Attack = value; } }

        public GameObject owner { get; set; } // 拥有者在Start里面被赋值

        [SerializeField] float m_AssaultInterval = 0.3f; // 攻击时间间隔
        public float assaultInterval { get { return m_AssaultInterval; } set { m_AssaultInterval = value; } }

        [SerializeField] float m_Thrust = 10; // 推力
        public float thrust { get { return m_Thrust; } set { m_Thrust = value; } }

        public float lastAssaultTime { get; set; } // 上次攻击时间

        private CommonTouchAttacker<TouchAttacker> commonTouchAttacker; // 公共实现

        private void Awake() { commonTouchAttacker = new CommonTouchAttacker<TouchAttacker>(this); }

        // 变量初始化
        private void Start() { commonTouchAttacker.Start(); }

        // 碰到玩家就造成伤害并推开
        private void OnCollisionEnter2D(Collision2D collision) { commonTouchAttacker.Assault(collision); }

        // 挨着玩家就造成伤害并推开
        private void OnCollisionStay2D(Collision2D collision) { commonTouchAttacker.Assault(collision); }

        // 推开受伤者
        public void Push(GameObject pushedObject, Vector2 contactPoint) { commonTouchAttacker.Push(pushedObject, contactPoint); }
    }
}