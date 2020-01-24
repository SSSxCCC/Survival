using UnityEngine;

public class CommonDropItem<T> where T : MonoBehaviour, IDropItem
{
    private T dropItem;

    public CommonDropItem(T dropItem)
    {
        this.dropItem = dropItem;
    }

    public void OnDestroy()
    {
        if (Random.value <= dropItem.chance)
        {
            dropItem.CreateDroppedItem();
        }
    }

    public GameObject CreateDroppedItem()
    {
        GameObject droppedItemPrefab = dropItem.droppedItemPrefabs[Random.Range(0, dropItem.droppedItemPrefabs.Length)];
        GameObject droppedItem = Object.Instantiate(droppedItemPrefab, dropItem.transform.position, Quaternion.identity);
        return droppedItem;
    }
}
