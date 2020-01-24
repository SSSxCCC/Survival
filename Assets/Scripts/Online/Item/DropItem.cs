using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class DropItem : NetworkBehaviour, IDropItem
    {
        [SerializeField] float m_Chance = 0.5f; // 掉落几率
        public float chance { get { return m_Chance; } set { m_Chance = value; } }

        [SerializeField] GameObject[] m_DroppedItemPrefabs; // 所有可能掉落的物品
        public GameObject[] droppedItemPrefabs { get { return m_DroppedItemPrefabs; } set { m_DroppedItemPrefabs = value; } }

        private CommonDropItem<DropItem> commonDropItem; // 公共实现

        private void Awake()
        {
            commonDropItem = new CommonDropItem<DropItem>(this);
        }

        // 死亡时有一定几率掉落物品
        [ServerCallback]
        public override void OnNetworkDestroy()
        {
            commonDropItem.OnDestroy();
        }

        // 创建掉落物品
        [Server]
        public void CreateDroppedItem()
        {
            GameObject droppedItem = commonDropItem.CreateDroppedItem();
            NetworkServer.Spawn(droppedItem);
        }

        // 静态方法掉落物品
        [Server]
        public static void Create(GameObject droppedItemPrefab, Vector2 position)
        {
            GameObject droppedItem = Instantiate(droppedItemPrefab, position, Quaternion.identity);
            NetworkServer.Spawn(droppedItem);
        }
    }
}