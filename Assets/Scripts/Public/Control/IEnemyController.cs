using UnityEngine;

/// <summary>
/// 敌对电脑控制者接口。
/// </summary>
public interface IEnemyController : IController
{
    GameObject targetPlayer { get; set; }

    void GoToward(Vector2 targetPosition);
}
