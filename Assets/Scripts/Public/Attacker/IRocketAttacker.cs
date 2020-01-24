using UnityEngine;

/// <summary>
/// 导弹接口。
/// </summary>
public interface IRocketAttacker : IAmmoAttacker
{
    float propulsion { get; set; }
    GameObject explosionPrefab { get; set; }
	
}
