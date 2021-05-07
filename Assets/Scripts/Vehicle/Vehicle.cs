using System;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Vehicle : NetworkBehaviour {
    [SyncVar] GameObject m_Driver; // 驾驶者
    public GameObject driver { get { return m_Driver; } set { m_Driver = value; } }

    // 玩家进入载具，无视此玩家与载具之间的碰撞
    [Server]
    public virtual void Enter(GameObject player) {
        RpcEnter(player);
    }
    [ClientRpc]
    private void RpcEnter(GameObject player) {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
    }

    // 玩家离开载具，重新使玩家与载具间的碰撞有效
    [Server]
    public virtual void Leave(GameObject player) {
        RpcLeave(player);
    }
    [ClientRpc]
    private void RpcLeave(GameObject player) {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
    }

    // 离开所有进入的玩家
    [Server]
    public abstract void LeaveAll();

    // 得到载具上的武器信息
    [Client]
    public virtual VehicleWeaponState GetWeaponState(GameObject player) {
        return null;
    }

    // 操作载具
    [Client]
    public abstract void Control(GameObject player, Vector2 axis);

    // 以下几个方法有关于玩家单位操作，返回值代表是否允许玩家单位自己进行相应的操作
    [Client] public abstract bool Move(GameObject player);
    [Client] public abstract bool Rotate(GameObject player);
    [Client] public abstract bool NextWeapon(GameObject player);
    [Client] public abstract bool Fire(GameObject player);
    
    /// <summary>
    /// 静态方法刷载具
    /// </summary>
    /// <param name="vehiclePrefab">刷的载具</param>
    /// <param name="position">刷的位置</param>
    /// <param name="angle">初始朝向角度</param>
    [Server]
    public static void Create(GameObject vehiclePrefab, Vector2 position, float angle) {
        GameObject vehicle = Instantiate(vehiclePrefab, position, Quaternion.Euler(0, 0, angle));
        NetworkServer.Spawn(vehicle);
    }
}



[Serializable]
public class VehicleWeaponState {
    public Sprite sprite;
    public Color color;
    public int numAmmo;

    public VehicleWeaponState(Sprite sprite, Color color, int numAmmo) {
        this.sprite = sprite;
        this.color = color;
        this.numAmmo = numAmmo;
    }
}
