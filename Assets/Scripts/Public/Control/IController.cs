using UnityEngine;

/// <summary>
/// 控制者/操作者接口。
/// </summary>
public interface IController
{
    float moveForce { get; set; }
    int obstacleLayerMask { get; }

    void LookAt(Vector2 point);
    void RotateTo(Vector2 direction);
    void MoveRotation(float angle);
    void Move(Vector2 force);
    bool HasObstacleTo(Vector2 point);
}
