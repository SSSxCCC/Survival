using UnityEngine;

/// <summary>
/// 射击武器接口。
/// </summary>
public interface IWeapon : IShooter
{
    WeaponType weaponType { get; set; }
    SpriteRenderer weaponRenderer { get; set; }
    Sprite weaponSprite { get; set; }
    int numAmmo { get; set; }
    GameObject owner { get; set; }

    void Fire();
    void CreateAmmo(Vector2 muzzlePosition, Vector2 normalizedDirection);
    void SetVisible(bool visible);
}
