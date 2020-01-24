using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class AmmoSaver : ObjectSaver
    {
        public override void LoadStringData(string stringData)
        {
            AmmoData ammoData = JsonUtility.FromJson<AmmoData>(stringData);

            IAmmoAttacker ammoAttacker = GetComponent<IAmmoAttacker>();
            ammoAttacker.attack = ammoData.attack;

            ownerIndex = ammoData.ownerIndex;

            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            rb2D.velocity = ammoData.linearVelocity;
            rb2D.angularVelocity = ammoData.angularVelocity;
        }

        private int ownerIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (ownerIndex >= 0) GetComponent<IAmmoAttacker>().owner = loadedObjectList[ownerIndex];
        }

        protected override string GetStringData()
        {
            IAmmoAttacker ammoAttacker = GetComponent<IAmmoAttacker>();
            int ownerIndex = ammoAttacker.owner != null ? ammoAttacker.owner.GetComponent<ObjectSaver>().tempIndex : -1;
            Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
            AmmoData ammoData = new AmmoData(ammoAttacker.attack, ownerIndex, rb2D.velocity, rb2D.angularVelocity);
            return JsonUtility.ToJson(ammoData);
        }

        [Serializable]
        private class AmmoData
        {
            public int attack;
            public int ownerIndex;
            public Vector2 linearVelocity;
            public float angularVelocity;

            public AmmoData(int attack, int ownerIndex, Vector2 linearVelocity, float angularVelocity)
            {
                this.attack = attack;
                this.ownerIndex = ownerIndex;
                this.linearVelocity = linearVelocity;
                this.angularVelocity = angularVelocity;
            }
        }
    }
}