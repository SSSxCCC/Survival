using UnityEngine;

/// <summary>
/// 玩家操作者接口。
/// </summary>
public interface IPlayerController : IController
{
    GameObject vehicle { get; set; }

    void EnterOrLeave();
}
