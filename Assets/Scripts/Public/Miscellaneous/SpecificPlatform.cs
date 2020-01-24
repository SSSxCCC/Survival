using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本脚本贴在某些只需要存在于特定平台的物体上，使其在其他平台自动销毁。
/// </summary>
public class SpecificPlatform : MonoBehaviour {
    
    public PlatformType existPlatformType; // 本物体只需要存在的平台类型

    private static List<RuntimePlatform> PC_Platforms = new List<RuntimePlatform> { RuntimePlatform.WindowsPlayer, RuntimePlatform.WindowsEditor, RuntimePlatform.OSXPlayer, RuntimePlatform.OSXEditor, RuntimePlatform.LinuxPlayer, RuntimePlatform.LinuxEditor };
    private static List<RuntimePlatform> Phone_Platforms = new List<RuntimePlatform> { RuntimePlatform.Android, RuntimePlatform.IPhonePlayer };
    private static Dictionary<PlatformType, List<RuntimePlatform>> platformLists = new Dictionary<PlatformType, List<RuntimePlatform>> { { PlatformType.PC, PC_Platforms}, { PlatformType.Phone, Phone_Platforms } };

    private void Awake()
    {
        if (!IsCurrent(existPlatformType))
        {
            DestroyImmediate(gameObject);
        }
    }

    /// <summary>
    /// 判断游戏当前运行平台是否是某类型的平台
    /// </summary>
    /// <param name="platformType">判断的平台类型</param>
    /// <returns>如果当前平台与参数平台类型匹配，则返回true，否则返回false</returns>
    public static bool IsCurrent(PlatformType platformType)
    {
        return platformLists[platformType].Contains(Application.platform);
    }
}

public enum PlatformType { PC, Phone }