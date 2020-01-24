using UnityEngine;

namespace Offline
{
    public class DroppedWeapon : MonoBehaviour, IDroppedWeapon
    {
        [SerializeField] GameObject m_WeaponPrefab; // 此掉落武器代表的武器
        public GameObject weaponPrefab { get { return m_WeaponPrefab; } set { m_WeaponPrefab = value; } }

        [SerializeField] int m_NumAmmo = 1; // 掉落武器包含的弹药数量
        public int numAmmo { get { return m_NumAmmo; } set { m_NumAmmo = value; } }

        private CommonDroppedWeapon<DroppedWeapon> commonDroppedWeapon; // 公共实现

        private void Awake() { commonDroppedWeapon = new CommonDroppedWeapon<DroppedWeapon>(this); }

        // 碰到东西了
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (commonDroppedWeapon.FindOwner(collider)) Destroy(gameObject); // 找到主人则摧毁自己
        }

        // 创建此掉落武器代表的武器实例
        public GameObject CreateWeapon(Vector3 position, float angle) { return commonDroppedWeapon.CreateWeapon(position, angle); }
    }
}