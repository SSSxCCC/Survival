using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class FloatboardSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            FloatboardData floatboardData = JsonUtility.FromJson<FloatboardData>(stringData);

            IState state = GetComponent<IState>();
            state.maxHealth = floatboardData.maxHealth;
            state.health = floatboardData.health;
            state.defense = floatboardData.defense;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = floatboardData.linearVelocity;
            rb2D.angularVelocity = floatboardData.angularVelocity;

            IFloatboard floatboard = GetComponent<IFloatboard>();
            floatboard.driverSeatIndex = floatboardData.driverSeatIndex;

            tempFloatboardData = floatboardData;
        }

        private FloatboardData tempFloatboardData;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            IFloatboard floatboard = GetComponent<IFloatboard>();
            if (tempFloatboardData.driverIndex >= 0) floatboard.driver = loadedObjectList[tempFloatboardData.driverIndex];
            if (tempFloatboardData.passengerIndex >= 0) floatboard.passenger = loadedObjectList[tempFloatboardData.passengerIndex];
        }

        protected override string GetStringData()
        {
            IState state = GetComponent<IState>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            IFloatboard floatboard = GetComponent<IFloatboard>();
            int driverIndex = floatboard.driver != null ? floatboard.driver.GetComponent<ObjectSaver>().tempIndex : -1;
            int passengerIndex = floatboard.passenger != null ? floatboard.passenger.GetComponent<ObjectSaver>().tempIndex : -1;
            FloatboardData floatboardData = new FloatboardData(state.maxHealth, state.health, state.defense, rb2D.velocity, rb2D.angularVelocity, floatboard.driverSeatIndex, driverIndex, passengerIndex);
            return JsonUtility.ToJson(floatboardData);
        }

        [Serializable]
        private class FloatboardData
        {
            public int maxHealth;
            public int health;
            public int defense;
            public Vector2 linearVelocity;
            public float angularVelocity;
            public int driverSeatIndex;
            public int driverIndex;
            public int passengerIndex;

            public FloatboardData(int maxHealth, int health, int defense, Vector2 linearVelocity, float angularVelocity, int driverSeatIndex, int driverIndex, int passengerIndex)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
                this.driverSeatIndex = driverSeatIndex;
                this.driverIndex = driverIndex;
                this.passengerIndex = passengerIndex;
            }
        }
    }
}