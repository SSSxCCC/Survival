using UnityEngine;
using UnityEngine.Networking;

public class EnemyWeapon : Shooter {
    [SerializeField] float m_AttackRange = 1f; // 攻击距离
    public float attackRange { get { return m_AttackRange; } set { m_AttackRange = value; } }

    private State state;
    private EnemyController enemyController;

    // 初始化引用
    private void Start() {
        state = GetComponent<State>();
        enemyController = GetComponent<EnemyController>();
    }

    // 自动射击
    [ServerCallback]
    private void Update() {
        // 必须自己活着且有目标且与目标的距离小于等于攻击距离且有足够的开火时间间隔
        if (state.health <= 0 || enemyController.targetPlayer == null
            || Vector2.Distance(enemyController.targetPlayer.transform.position, transform.position) > attackRange
            || Time.time - lastFireTime < fireInterval) {
            return;
        }

        // 开火
        var normalizedDirection = (muzzle.position - transform.position).normalized;
        var ammo = Instantiate(ammoPrefab, muzzle.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(normalizedDirection.y, normalizedDirection.x)));
        var ammoAttacker = ammo.GetComponent<AmmoAttacker>();
        ammoAttacker.attack = attack;
        ammoAttacker.owner = gameObject;
        if (ammoSpeed > 0) ammo.GetComponent<Rigidbody2D>().velocity = normalizedDirection * ammoSpeed;
        lastFireTime = Time.time;
        NetworkServer.Spawn(ammo);
    }
}
