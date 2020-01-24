using System;
using UnityEngine;

namespace Offline
{
    [CreateAssetMenu(menuName = "Mission/mission 1-1")]
    public class Mission1c1 : Mission
    {
        // 任务设定值，不要在代码中更改。
        public int sumEnemy = 1;
        // ----------------------------

        private int remainingEnemy;

        protected override void OnStart()
        {
            remainingEnemy = sumEnemy;
        }

        protected override void Initialize()
        {
            GameManager.singleton.Death += OnDeath;
        }

        protected override void OnComplete()
        {
            GameManager.singleton.Death += OnDeath;
        }

        private void OnDeath(GameObject deadObject)
        {
            if (deadObject.CompareTag("Enemy")) remainingEnemy--;
            if (remainingEnemy <= 0) Complete();
        }

        protected override void SetMissionData(string missionData)
        {
            remainingEnemy = int.Parse(missionData);
        }

        protected override string GetMissionData()
        {
            return remainingEnemy.ToString();
        }
    }
}