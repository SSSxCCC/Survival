using UnityEngine;

/// <summary>
/// 汽车接口。
/// </summary>
public interface ICar : IVehicle
{
    Transform[] seats { get; set; }
    GameObject passenger1 { get; set; }
    GameObject passenger2 { get; set; }
    GameObject passenger3 { get; set; }
    float rotateTorque { get; set; }
    float powerForce { get; set; }
}
