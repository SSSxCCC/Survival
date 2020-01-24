using UnityEngine;

/// <summary>
/// 飞行器接口。
/// </summary>
public interface IAircraft : IVehicle
{
    Transform[] seats { get; set; }
    GameObject passenger1 { get; set; }
    GameObject passenger2 { get; set; }
    float rotateTorque { get; set; }
    float powerForce { get; set; }
}
