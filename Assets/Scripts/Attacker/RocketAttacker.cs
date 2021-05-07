using UnityEngine;
using UnityEngine.Networking;

public class RocketAttacker : AmmoAttacker {
    [SerializeField] float m_Propulsion; // 推进力
    public float propulsion { get { return m_Propulsion; } set { m_Propulsion = value; } }

    [SerializeField] GameObject m_ExplosionPrefab; // 爆炸的prefab
    public GameObject explosionPrefab { get { return m_ExplosionPrefab; } set { m_ExplosionPrefab = value; } }

    private Rigidbody2D rb2D;

    protected override void Start() {
        base.Start();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // 施加推进力
    [ServerCallback]
    private void FixedUpdate() {
        float radianAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        rb2D.AddForce(new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * propulsion);
    }

    // 不需要直接造成伤害
    [Server]
    public override void OnAttack(State state, Collision2D collision) { }

    // 产生爆炸造成伤害
    [Server]
    public override void OnDeath(Collision2D collision) {
        if (Random.value < 0.1f) // 劣质的导弹
            return;

        // 创造爆炸
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Explosion exp = explosion.GetComponent<Explosion>();
        exp.attack = attack;
        exp.owner = owner;
        NetworkServer.Spawn(explosion);
        NetworkServer.Destroy(gameObject);
    }
}
