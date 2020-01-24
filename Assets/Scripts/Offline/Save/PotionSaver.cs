using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class PotionSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            PotionData potionData = JsonUtility.FromJson<PotionData>(stringData);
            IPotion potion = GetComponent<IPotion>();

            potion.maxHealth = potionData.maxHealth;
            potion.health = potionData.health;
            potion.defense = potionData.defense;
        }

        public override void LoadMoreData(List<GameObject> loadedObjectList) { }

        protected override string GetStringData()
        {
            IPotion potion = GetComponent<IPotion>();
            PotionData potionData = new PotionData(potion.maxHealth, potion.health, potion.defense);
            return JsonUtility.ToJson(potionData);
        }

        [Serializable]
        private class PotionData
        {
            public int maxHealth;
            public int health;
            public int defense;

            public PotionData(int maxHealth, int health, int defense)
            {
                this.maxHealth = maxHealth;
                this.health = health;
                this.defense = defense;
            }
        }
    }
}