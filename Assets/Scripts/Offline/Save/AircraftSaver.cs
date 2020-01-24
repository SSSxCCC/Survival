using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class AircraftSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            AircraftData aircraftData = JsonUtility.FromJson<AircraftData>(stringData);

            IState state = GetComponent<IState>();
            state.maxHealth = aircraftData.maxHealth;
            state.health = aircraftData.health;
            state.defense = aircraftData.defense;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = aircraftData.linearVelocity;
            rb2D.angularVelocity = aircraftData.angularVelocity;

            tempAircraftData = aircraftData;
        }

        private AircraftData tempAircraftData;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            IAircraft aircraft = GetComponent<IAircraft>();
            if (tempAircraftData.driverIndex >= 0) aircraft.driver = loadedObjectList[tempAircraftData.driverIndex];
            if (tempAircraftData.passenger1Index >= 0) aircraft.passenger1 = loadedObjectList[tempAircraftData.passenger1Index];
            if (tempAircraftData.passenger2Index >= 0) aircraft.passenger2 = loadedObjectList[tempAircraftData.passenger2Index];
        }

        protected override string GetStringData()
        {
            IState state = GetComponent<IState>();
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            IAircraft aircraft = GetComponent<IAircraft>();
            int driverIndex = aircraft.driver != null ? aircraft.driver.GetComponent<ObjectSaver>().tempIndex : -1;
            int passenger1Index = aircraft.passenger1 != null ? aircraft.passenger1.GetComponent<ObjectSaver>().tempIndex : -1;
            int passenger2Index = aircraft.passenger2 != null ? aircraft.passenger2.GetComponent<ObjectSaver>().tempIndex : -1;
            AircraftData aircraftData = new AircraftData(state.maxHealth, state.health, state.defense, rb2D.velocity, rb2D.angularVelocity, driverIndex, passenger1Index, passenger2Index);
            return JsonUtility.ToJson(aircraftData);
        }

        [Serializable]
        private class AircraftData
        {
            public int maxHealth;
            public int health;
            public int defense;
            public Vector2 linearVelocity;
            public float angularVelocity;
            public int driverIndex;
            public int passenger1Index;
            public int passenger2Index;

            public AircraftData(int maxHealth, int health, int defense, Vector2 linearVelocity, float angularVelocity, int driverIndex, int passenger1Index, int passenger2Index)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
                this.driverIndex = driverIndex;
                this.passenger1Index = passenger1Index;
                this.passenger2Index = passenger2Index;
            }
        }
    }
}