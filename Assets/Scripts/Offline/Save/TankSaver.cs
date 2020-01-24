using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class TankSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            TankData tankData = JsonUtility.FromJson<TankData>(stringData);

            IState state = GetComponent<IState>();
            state.maxHealth = tankData.maxHealth;
            state.health = tankData.health;
            state.defense = tankData.defense;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = tankData.linearVelocity;
            rb2D.angularVelocity = tankData.angularVelocity;

            driverIndex = tankData.driverIndex;
        }

        private int driverIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (driverIndex >= 0)
                GetComponent<ITank>().driver = loadedObjectList[driverIndex];
        }

        protected override string GetStringData()
        {
            IState state = GetComponent<IState>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            ITank tank = GetComponent<ITank>();
            int driverIndex = tank.driver != null ? tank.driver.GetComponent<ObjectSaver>().tempIndex : -1;
            TankData tankData = new TankData(state.maxHealth, state.health, state.defense, rb2D.velocity, rb2D.angularVelocity, driverIndex);
            return JsonUtility.ToJson(tankData);
        }

        [Serializable]
        private class TankData
        {
            public int maxHealth;
            public int health;
            public int defense;
            public Vector2 linearVelocity;
            public float angularVelocity;
            public int driverIndex;

            public TankData(int maxHealth, int health, int defense, Vector2 linearVelocity, float angularVelocity, int driverIndex)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
                this.driverIndex = driverIndex;
            }
        }
    }
}