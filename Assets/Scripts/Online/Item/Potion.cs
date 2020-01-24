using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class Potion : NetworkBehaviour, IPotion
    {
        [SerializeField] int m_MaxHealth; // 此药水可以增加的最大生命值
        public int maxHealth { get { return m_MaxHealth; } set { m_MaxHealth = value; } }

        [SerializeField] int m_Health; // 此药水可以增加的生命值
        public int health { get { return m_Health; } set { m_Health = value; } }

        [SerializeField] int m_Defense; // 次药水可以增加的防御力
        public int defense { get { return m_Defense; } set { m_Defense = value; } }

        private CommonPotion<Potion> commonPotion; // 公共实现

        private void Awake()
        {
            commonPotion = new CommonPotion<Potion>(this);
        }

        // 被玩家捡到增加相应状态属性
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (commonPotion.FindOwner(collider)) NetworkServer.Destroy(gameObject);
        }
    }
}