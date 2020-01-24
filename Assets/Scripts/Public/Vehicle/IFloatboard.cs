using UnityEngine;

/// <summary>
/// 浮板接口。
/// </summary>
public interface IFloatboard : IVehicle
{
    Transform[] seats { get; set; }
    int driverSeatIndex { get; set; }
    GameObject passenger { get; set; }
    float rotateTorque { get; set; }
    float powerForce { get; set; }
}
