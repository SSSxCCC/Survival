using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class StateSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            StateData stateData = JsonUtility.FromJson<StateData>(stringData);
            IState state = GetComponent<IState>();

            state.maxHealth = stateData.maxHealth;
            state.health = stateData.health;
            state.defense = stateData.defense;
        }

        public override void LoadMoreData(List<GameObject> loadedObjectList) { }

        protected override string GetStringData()
        {
            IState state = GetComponent<IState>();
            StateData stateData = new StateData(state.maxHealth, state.health, state.defense);
            return JsonUtility.ToJson(stateData);
        }

        [Serializable]
        private class StateData
        {
            public int maxHealth;
            public int health;
            public int defense;

            public StateData(int maxHealth, int health, int defense)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
            }
        }
    }
}