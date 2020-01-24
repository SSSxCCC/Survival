using UnityEngine;

/// <summary>
/// 射击者通用接口。
/// </summary>
public interface IShooter
{
    Transform muzzle { get; set; }
    GameObject ammoPrefab { get; set; }
    int attack { get; set; }
    float fireInterval { get; set; }
    float ammoSpeed { get; set; }
    float lastFireTime { get; set; }
}
