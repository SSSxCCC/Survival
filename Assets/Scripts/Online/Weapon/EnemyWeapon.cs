﻿using UnityEngine;
using UnityEngine.Networking;

namespace Online
{
    public class EnemyWeapon : NetworkBehaviour, IEnemyWeapon
    {
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

        [SerializeField] float m_AttackRange = 1f; // 攻击距离
        public float attackRange { get { return m_AttackRange; } set { m_AttackRange = value; } }

        float m_LastFireTime; // 上次开火时间
        public float lastFireTime { get { return m_LastFireTime; } set { m_LastFireTime = value; } }

        private CommonEnemyWeapon<EnemyWeapon> commonEnemyWeapon; // 公共实现

        private void Awake()
        {
            commonEnemyWeapon = new CommonEnemyWeapon<EnemyWeapon>(this);
        }

        // 初始化引用
        private void Start()
        {
            commonEnemyWeapon.Start();
        }

        // 自动射击
        [ServerCallback]
        private void Update()
        {
            GameObject ammo = commonEnemyWeapon.CreateAmmo();
            if (ammo != null) NetworkServer.Spawn(ammo);
        }
    }
}