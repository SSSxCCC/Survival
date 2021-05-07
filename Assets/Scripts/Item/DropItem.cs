using UnityEngine;
using UnityEngine.Networking;

public class DropItem : NetworkBehaviour {
    [SerializeField] float m_Chance = 0.5f; // 掉落几率
    public float chance { get { return m_Chance; } set { m_Chance = value; } }

    [SerializeField] GameObject[] m_DroppedItemPrefabs; // 所有可能掉落的物品
    public GameObject[] droppedItemPrefabs { get { return m_DroppedItemPrefabs; } set { m_DroppedItemPrefabs = value; } }

    // 死亡时有一定几率掉落物品
    [ServerCallback]
    public override void OnNetworkDestroy() {
        if (Random.value <= chance) {
            CreateDroppedItem();
        }
    }

    // 创建掉落物品
    [Server]
    public void CreateDroppedItem() {
        GameObject droppedItemPrefab = droppedItemPrefabs[Random.Range(0, droppedItemPrefabs.Length)];
        GameObject droppedItem = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(droppedItem);
    }

    // 静态方法掉落物品
    [Server]
    public static void Create(GameObject droppedItemPrefab, Vector2 position) {
        GameObject droppedItem = Instantiate(droppedItemPrefab, position, Quaternion.identity);
        NetworkServer.Spawn(droppedItem);
    }
}
