using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class CarSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            CarData carData = JsonUtility.FromJson<CarData>(stringData);

            IState state = GetComponent<IState>();
            state.maxHealth = carData.maxHealth;
            state.health = carData.health;
            state.defense = carData.defense;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = carData.linearVelocity;
            rb2D.angularVelocity = carData.angularVelocity;

            tempCarData = carData;
        }

        private CarData tempCarData;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            ICar car = GetComponent<ICar>();
            if (tempCarData.driverIndex >= 0) car.driver = loadedObjectList[tempCarData.driverIndex];
            if (tempCarData.passenger1Index >= 0) car.passenger1 = loadedObjectList[tempCarData.passenger1Index];
            if (tempCarData.passenger2Index >= 0) car.passenger2 = loadedObjectList[tempCarData.passenger2Index];
            if (tempCarData.passenger3Index >= 0) car.passenger3 = loadedObjectList[tempCarData.passenger3Index];
        }

        protected override string GetStringData()
        {
            IState state = GetComponent<IState>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            ICar car = GetComponent<ICar>();
            int driverIndex = car.driver != null ? car.driver.GetComponent<ObjectSaver>().tempIndex : -1;
            int passenger1Index = car.passenger1 != null ? car.passenger1.GetComponent<ObjectSaver>().tempIndex : -1;
            int passenger2Index = car.passenger2 != null ? car.passenger2.GetComponent<ObjectSaver>().tempIndex : -1;
            int passenger3Index = car.passenger3 != null ? car.passenger3.GetComponent<ObjectSaver>().tempIndex : -1;
            CarData carData = new CarData(state.maxHealth, state.health, state.defense, rb2D.velocity, rb2D.angularVelocity, driverIndex, passenger1Index, passenger2Index, passenger3Index);
            return JsonUtility.ToJson(carData);
        }

        [Serializable]
        private class CarData
        {
            public int maxHealth;
            public int health;
            public int defense;
            public Vector2 linearVelocity;
            public float angularVelocity;
            public int driverIndex;
            public int passenger1Index;
            public int passenger2Index;
            public int passenger3Index;

            public CarData(int maxHealth, int health, int defense, Vector2 linearVelocity, float angularVelocity, int driverIndex, int passenger1Index, int passenger2Index, int passenger3Index)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
                this.driverIndex = driverIndex;
                this.passenger1Index = passenger1Index;
                this.passenger2Index = passenger2Index;
                this.passenger3Index = passenger3Index;
            }
        }
    }
}