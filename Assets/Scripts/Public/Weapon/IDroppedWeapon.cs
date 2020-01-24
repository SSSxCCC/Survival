using UnityEngine;

/// <summary>
/// 掉落的武器接口。
/// </summary>
public interface IDroppedWeapon
{
    GameObject weaponPrefab { get; set; }
    int numAmmo { get; set; }
    
    GameObject CreateWeapon(Vector3 position, float angle);
}
