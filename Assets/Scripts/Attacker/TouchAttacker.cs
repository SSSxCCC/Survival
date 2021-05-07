using UnityEngine;
using UnityEngine.Networking;

public class TouchAttacker : NetworkBehaviour {
    [SerializeField] int m_Attack = 1; // 攻击力
    public int attack { get { return m_Attack; } set { m_Attack = value; } }

    //public GameObject owner { get; set; } // 拥有者在Start里面被赋值

    [SerializeField] float m_AssaultInterval = 0.3f; // 攻击时间间隔
    public float assaultInterval { get { return m_AssaultInterval; } set { m_AssaultInterval = value; } }

    [SerializeField] float m_Thrust = 10; // 推力
    public float thrust { get { return m_Thrust; } set { m_Thrust = value; } }

    float m_LastAssaultTime; // 上次攻击时间
    public float lastAssaultTime { get { return m_LastAssaultTime; } set { m_LastAssaultTime = value; } }

    private State myState;
    private ContactPoint2D[] contacts = new ContactPoint2D[1];

    // 变量初始化
    private void Start() {
        //owner = gameObject;
        myState = GetComponent<State>();
    }

    // 碰到玩家就造成伤害并推开
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision) {
        Assault(collision);
    }

    // 挨着玩家就造成伤害并推开
    [ServerCallback]
    private void OnCollisionStay2D(Collision2D collision) {
        Assault(collision);
    }

    [Server]
    private void Assault(Collision2D collision) {
        if (myState != null && myState.health <= 0) return; // 自己不能是死的
        if (Time.time - lastAssaultTime < assaultInterval) return; // 攻击间隔检查

        var state = collision.gameObject.GetComponent<State>();
        if (AttackerUtility.ShouldDamage(gameObject, state)) {
            state.TakeAttack(attack); // 造成伤害
            lastAssaultTime = Time.time;
            if (collision.GetContacts(contacts) > 0) {
                RpcPush(collision.gameObject, contacts[0].point); // 推开受伤者
                state.DamagedEffect(contacts[0].point); // 产生受伤特效
            }
        }
    }

    // 推开受伤者
    [ClientRpc]
    public void RpcPush(GameObject pushedObject, Vector2 contactPoint) {
        if (pushedObject == null) return;
        Vector2 pushForce = (contactPoint - (Vector2)transform.position).normalized * thrust;
        pushedObject.GetComponent<Rigidbody2D>().AddForceAtPosition(pushForce, contactPoint, ForceMode2D.Impulse);
    }
}
