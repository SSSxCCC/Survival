using UnityEngine;
using UnityEngine.Networking;

public class ShotgunBulletAttacker : AmmoAttacker {
    [SerializeField] int m_ExtraNumBullets = 10; // 额外子弹个数
    public int extraNumBullets { get { return m_ExtraNumBullets; } set { m_ExtraNumBullets = value; } }

    bool m_IsHost = true; // 宿主子弹
    public bool isHost { get { return m_IsHost; } set { m_IsHost = value; } }

    [SyncVar] int m_ShotgunBulletId; // 本发散弹的id（如果没有SyncVar，主机切换时可能会有点问题）
    public int shotgunBulletId { get { return m_ShotgunBulletId; } set { m_ShotgunBulletId = value; } }

    private static int ID; // 定义每发散弹的id，为了使其不会互相伤害

    protected override void Start() {
        base.Start();
        if (!isServer) return;
        if (isHost) {
            CreateBullets();
        }
    }

    // 宿主子弹复制自己生成所有额外子弹
    [Server]
    private void CreateBullets() {
        shotgunBulletId = ID++; // 得到一个唯一的id

        // 产生剩余子弹
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        Vector3 vertical = new Vector3(-rb2D.velocity.y, rb2D.velocity.x, 0).normalized * 0.01f;
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < extraNumBullets; i++)
        {
            offset *= -1;
            if (i % 2 == 0) offset += vertical;
            var bullet = Instantiate(gameObject, transform.position + offset, transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = rb2D.velocity;
            var bulletAttacker = bullet.GetComponent<ShotgunBulletAttacker>();
            bulletAttacker.attack = attack;
            bulletAttacker.owner = owner;
            bulletAttacker.isHost = false;
            bulletAttacker.shotgunBulletId = shotgunBulletId;
            NetworkServer.Spawn(bullet);
        }
    }

    // 直接攻击
    [Server]
    public override void OnAttack(State state, Collision2D collision) {
        OnDirectAttack(state, collision);
    }

    // 不会与同一次发出的其它散弹互相伤害
    [Server]
    public override void OnDeath(Collision2D collision) {
        var shotgunBulletAttacker = collision.gameObject.GetComponent<ShotgunBulletAttacker>();
        if (shotgunBulletAttacker == null || shotgunBulletId != shotgunBulletAttacker.shotgunBulletId) {
            NetworkServer.Destroy(gameObject);
        }
    }
}
