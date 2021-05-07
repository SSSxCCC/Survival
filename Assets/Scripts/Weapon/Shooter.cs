using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 射击器基类。
/// </summary>
public abstract class Shooter : NetworkBehaviour {
    [SerializeField] GameObject m_AmmoPrefab; // 射出来的东西
    public GameObject ammoPrefab { get { return m_AmmoPrefab; } set { m_AmmoPrefab = value; } }

    [SerializeField] Transform m_Muzzle; // 射击口
    public Transform muzzle { get { return m_Muzzle; } set { m_Muzzle = value; } }

    [SerializeField] int m_Attack = 1; // 攻击力
    public int attack { get { return m_Attack; } set { m_Attack = value; } }

    [SerializeField] float m_FireInterval; // 开火最短间隔时间
    public float fireInterval { get { return m_FireInterval; } set { m_FireInterval = value; } }

    [SerializeField] float m_AmmoSpeed = 1f; // 弹速
    public float ammoSpeed { get { return m_AmmoSpeed; } set { m_AmmoSpeed = value; } }

    float m_LastFireTime; // 上次开火时间
    public float lastFireTime { get { return m_LastFireTime; } set { m_LastFireTime = value; } }
}