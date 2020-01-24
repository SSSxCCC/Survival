using UnityEngine;

/// <summary>
/// 载具接口。
/// </summary>
public interface IVehicle
{
    GameObject driver { get; set; }

    void Enter(GameObject player);
    void Leave(GameObject player);
    void LeaveAll();
    VehicleWeaponState GetWeaponState(GameObject player);
    void Control(GameObject player, Vector2 axis);

    // 以下几个方法有关于玩家单位操作，返回值代表是否允许玩家单位自己进行相应的操作
    bool Move(GameObject player);
    bool Rotate(GameObject player);
    bool NextWeapon(GameObject player);
    bool Fire(GameObject player);
}
