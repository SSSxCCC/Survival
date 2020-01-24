using UnityEngine;

/// <summary>
/// 死亡掉落物品接口。
/// </summary>
public interface IDropItem
{
    float chance { get; set; }
    GameObject[] droppedItemPrefabs { get; set; }

    void CreateDroppedItem();
}