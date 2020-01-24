using UnityEngine;

public class CommonEnemyWeapon<T> where T : MonoBehaviour, IEnemyWeapon
{
    private T enemyWeapon;

    private IState state;
    private IEnemyController enemyController;

    public CommonEnemyWeapon(T enemyWeapon)
    {
        this.enemyWeapon = enemyWeapon;
    }

    public void Start()
    {
        state = enemyWeapon.GetComponent<IState>();
        enemyController = enemyWeapon.GetComponent<IEnemyController>();
    }

    public GameObject CreateAmmo()
    {
        // 必须自己活着且有目标且与目标的距离小于等于攻击距离且有足够的开火时间间隔
        if (state.health <= 0 || enemyController.targetPlayer == null
            || Vector2.Distance(enemyController.targetPlayer.transform.position, enemyWeapon.transform.position) > enemyWeapon.attackRange
            || Time.time - enemyWeapon.lastFireTime < enemyWeapon.fireInterval)
        {
            return null;
        }

        // 开火
        Vector2 normalizedDirection = (enemyWeapon.muzzle.position - enemyWeapon.transform.position).normalized;
        GameObject ammo = Object.Instantiate(enemyWeapon.ammoPrefab, enemyWeapon.muzzle.position, Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(normalizedDirection.y, normalizedDirection.x)));
        IAmmoAttacker ammoAttacker = ammo.GetComponent<IAmmoAttacker>();
        ammoAttacker.attack = enemyWeapon.attack;
        ammoAttacker.owner = enemyWeapon.gameObject;
        if (enemyWeapon.ammoSpeed > 0) ammo.GetComponent<Rigidbody2D>().velocity = normalizedDirection * enemyWeapon.ammoSpeed;
        enemyWeapon.lastFireTime = Time.time;

        return ammo;
    }
}
