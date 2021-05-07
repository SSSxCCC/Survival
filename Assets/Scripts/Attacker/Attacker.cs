using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 攻击者基类。
/// </summary>
public abstract class Attacker : NetworkBehaviour {
    [SyncVar] int m_Attack; // 攻击力由发射器决定
    public int attack { get { return m_Attack; } set { m_Attack = value; } }

    [SyncVar] GameObject m_Owner; // 拥有者在被创建时被赋值
    public GameObject owner { get { return m_Owner; } set { m_Owner = value; } }
}
