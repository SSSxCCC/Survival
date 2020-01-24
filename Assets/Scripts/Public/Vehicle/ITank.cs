using UnityEngine;

/// <summary>
/// 坦克接口。
/// </summary>
public interface ITank : IVehicle, IShooter
{
    Transform seat { get; set; }
    Transform cannon { get; set; }
    float rotateTorque { get; set; }
    float powerForce { get; set; }
}
