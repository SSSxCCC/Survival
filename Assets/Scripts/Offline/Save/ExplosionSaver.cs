using System;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class ExplosionSaver : ObjectSaver
    {
        public ParticleSystem explosionEffect;

        public override void LoadStringData(string stringData)
        {
            ExplosionData explosionData = JsonUtility.FromJson<ExplosionData>(stringData);

            IExplosion explosion = GetComponent<IExplosion>();
            explosion.attack = explosionData.attack;

            ownerIndex = explosionData.ownerIndex;

            DestroyByTime destroyByTime = GetComponent<DestroyByTime>();
            float passedTime = destroyByTime.lifeTime - explosionData.remainingTime;
            destroyByTime.lifeTime = explosionData.remainingTime;
            explosionEffect.Simulate(passedTime);
            explosionEffect.Play();
        }

        private int ownerIndex;

        public override void LoadMoreData(List<GameObject> loadedObjectList)
        {
            if (ownerIndex >= 0) GetComponent<IExplosion>().owner = loadedObjectList[ownerIndex];
        }

        protected override string GetStringData()
        {
            IExplosion explosion = GetComponent<IExplosion>();
            int ownerIndex = explosion.owner != null ? explosion.owner.GetComponent<ObjectSaver>().tempIndex : -1;
            DestroyByTime destroyByTime = GetComponent<DestroyByTime>();
            ExplosionData explosionData = new ExplosionData(explosion.attack, ownerIndex, destroyByTime.remainingTime);
			return JsonUtility.ToJson(explosionData);
        }

        [Serializable]
        private class ExplosionData
        {
			public int attack;
            public int ownerIndex;
            public float remainingTime;

            public ExplosionData(int attack, int ownerIndex, float remainingTime)
            {
                this.attack = attack;
                this.ownerIndex = ownerIndex;
                this.remainingTime = remainingTime;
            }
        }
    }
}