using UnityEngine;

public class CommonShotgunBulletAttacker<T> where T : MonoBehaviour, IShotgunBulletAttacker
{
    private static int ID; // 定义每发散弹的id，为了使其不会互相伤害

    private T shotgunBulletAttacker;

    public CommonShotgunBulletAttacker(T shotgunBulletAttacker)
    {
        this.shotgunBulletAttacker = shotgunBulletAttacker;
    }

    public GameObject[] Start()
    {
        if (!shotgunBulletAttacker.isHost) return null;
        
        shotgunBulletAttacker.shotgunBulletId = ID++; // 得到一个唯一的id

        // 产生剩余子弹
        Rigidbody2D rb2D = shotgunBulletAttacker.GetComponent<Rigidbody2D>();
        Vector3 vertical = new Vector3(-rb2D.velocity.y, rb2D.velocity.x, 0).normalized * 0.01f;
        Vector3 offset = Vector3.zero;
        GameObject[] bullets = new GameObject[shotgunBulletAttacker.extraNumBullets];
        for (int i = 0; i < bullets.Length; i++)
        {
            offset *= -1;
            if (i % 2 == 0) offset += vertical;
            bullets[i] = Object.Instantiate(shotgunBulletAttacker.gameObject, shotgunBulletAttacker.transform.position + offset, shotgunBulletAttacker.transform.rotation);
            bullets[i].GetComponent<Rigidbody2D>().velocity = rb2D.velocity;
            IShotgunBulletAttacker bullet = bullets[i].GetComponent<IShotgunBulletAttacker>();
            bullet.attack = shotgunBulletAttacker.attack;
            bullet.owner = shotgunBulletAttacker.owner;
            bullet.isHost = false;
            bullet.shotgunBulletId = shotgunBulletAttacker.shotgunBulletId;
        }

        return bullets;
    }

    public bool ShouldDeath(Collision2D collision)
    {
        IShotgunBulletAttacker shotgunBulletAttacker = collision.gameObject.GetComponent<IShotgunBulletAttacker>();
        if (shotgunBulletAttacker == null || shotgunBulletAttacker.shotgunBulletId != shotgunBulletAttacker.shotgunBulletId)
        {
            return true;
        }
        return false;
    }
}
