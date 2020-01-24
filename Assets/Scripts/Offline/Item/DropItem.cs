using UnityEngine;

namespace Offline
{
    public class DropItem : MonoBehaviour, IDropItem
    {
        [SerializeField] float m_Chance = 0.5f; // 掉落几率
        public float chance { get { return m_Chance; } set { m_Chance = value; } }

        [SerializeField] GameObject[] m_DroppedItemPrefabs; // 所有可能掉落的物品
        public GameObject[] droppedItemPrefabs { get { return m_DroppedItemPrefabs; } set { m_DroppedItemPrefabs = value; } }

        private CommonDropItem<DropItem> commonDropItem; // 公共实现

        private void Awake() { commonDropItem = new CommonDropItem<DropItem>(this); }

        // 死亡时有一定几率掉落物品
        private void OnDestroy() { commonDropItem.OnDestroy(); }

        // 创建掉落物品
        public void CreateDroppedItem() { commonDropItem.CreateDroppedItem(); }

        // 静态方法掉落物品
        public static void Create(GameObject droppedItemPrefab, Vector2 position)
        {
            Instantiate(droppedItemPrefab, position, Quaternion.identity);
        }
    }
}